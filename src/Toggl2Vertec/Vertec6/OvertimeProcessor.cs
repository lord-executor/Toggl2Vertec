using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Toggl2Vertec.Configuration;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Vertec6.Api;
using Toggl2Vertec.Vertec6.Requests;

namespace Toggl2Vertec.Vertec6;

public class OvertimeProcessor
{
    private readonly ICliLogger _logger;
    private readonly CredentialStore _credStore;
    private readonly XmlApiClient _xmlApiClient;
    private readonly Settings _settings;

    public OvertimeProcessor(
        ICliLogger logger,
        CredentialStore credStore,
        XmlApiClient xmlApiClient,
        Settings settings
    )
    {
        _logger = logger;
        _credStore = credStore;
        _xmlApiClient = xmlApiClient;
        _settings = settings;
    }

    public void Process(int month)
    {
        // only support current year for now
        var dateStart = new DateTime(DateTime.Today.Year, month, 1);
        var lastDayMonth = DateTime.DaysInMonth(dateStart.Year, month);
        var dateEnd = new DateTime(dateStart.Year, month, lastDayMonth);
        
        _xmlApiClient.Authenticate().Wait();
        var monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);
        _logger.LogInfo($"Gathering overtime-data for {monthName}");

        var ownerId = new GetUserId(_credStore.VertecCredentials.UserName).Execute(_xmlApiClient);
        var monthTargetTimeInMinutes = new GetTargetTime(dateStart, dateEnd, ownerId).Execute(_xmlApiClient);
        var monthWorkTimeInMinutes = new GetWorkTime(dateStart, dateEnd, ownerId).Execute(_xmlApiClient);
        var monthHolidayTime = TimeSpan.FromMinutes(new GetHolidayTime(dateStart, dateEnd, ownerId).Execute(_xmlApiClient));

        var deltaTargetToActual = TimeSpan.FromMinutes(monthTargetTimeInMinutes - monthWorkTimeInMinutes);
        var monthTargetDuration = TimeSpan.FromMinutes(monthTargetTimeInMinutes);
        var monthWorkTimeDuration = TimeSpan.FromMinutes(monthWorkTimeInMinutes);

        _logger.LogContent($"Summary for {monthName}:");
        var hoursRequired = $" - Hours required:\t{FormatTimeSpan(monthTargetDuration)}";
        var hoursWorked = $" - Hours worked:\t{FormatTimeSpan(monthWorkTimeDuration)} (work: {FormatTimeSpan(monthWorkTimeDuration.Subtract(monthHolidayTime))}, holidays: {FormatTimeSpan(monthHolidayTime)})";
        _logger.LogContent(hoursRequired);
        _logger.LogContent(hoursWorked);
        _logger.LogContent($" - {(deltaTargetToActual.TotalMinutes < 0 ? "Overtime:\t\t+" : "Undertime:\t\t-")}{FormatTimeSpan(deltaTargetToActual)}");
        
        if (month != DateTime.Today.Month && DateTime.Today.Day != lastDayMonth)
        {
            return;
        }
        
        // Can do some more fun if current month and there is still time
        var tomorrow = DateTime.Today.AddDays(1);
        var employmentFactorDailyTargetDuration = TimeSpan.FromMinutes(new GetEmploymentFactor(DateTime.Today, ownerId).Execute(_xmlApiClient) * _settings.Company.DayWorkMinutes);
        
        var remainingTargetTime = TimeSpan.FromMinutes(new GetTargetTime(tomorrow, dateEnd, ownerId).Execute(_xmlApiClient));
        var workedToDate =
            TimeSpan.FromMinutes(new GetWorkTime(dateStart, DateTime.Today, ownerId).Execute(_xmlApiClient));
        var plannedHolidays =
            TimeSpan.FromMinutes(new GetHolidayTime(tomorrow, dateEnd, ownerId).Execute(_xmlApiClient));
        remainingTargetTime = remainingTargetTime.Subtract(plannedHolidays);
        var remainingWorkdays = remainingTargetTime.TotalMinutes / employmentFactorDailyTargetDuration.TotalMinutes;
        var expectedWorkingHours = TimeSpan.FromMinutes(remainingWorkdays * employmentFactorDailyTargetDuration.TotalMinutes);
        var expectedDelta = workedToDate
            .Add(plannedHolidays)
            .Add(expectedWorkingHours)
            .Subtract(monthTargetDuration);

        _logger.LogContent($"\nGetting to zero in {monthName}");
        _logger.LogContent($" - Still need to work:\t\t\t{FormatTimeSpan(remainingTargetTime)} (excl. holidays)");
        _logger.LogContent($" - Remaining workdays:\t\t\t{remainingWorkdays} (excl. holidays)");
        _logger.LogContent($" - Expected hours until end of month:\t{FormatTimeSpan(expectedWorkingHours)} (working {FormatTimeSpan(employmentFactorDailyTargetDuration)} per day)");
        _logger.LogContent($" - Expected {(expectedDelta.TotalMinutes > 0 ? "overtime:\t\t\t+" : "undertime\t\t\t-")}{FormatTimeSpan(expectedDelta)}");
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return $"{(int)Math.Floor(Math.Abs(timeSpan.TotalHours))}:{Math.Abs(timeSpan.Minutes):00}";
    }
}