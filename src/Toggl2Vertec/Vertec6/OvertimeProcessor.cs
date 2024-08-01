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
        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        _logger.LogInfo($"Gathering overtime-data for {monthName}");

        long ownerId = new GetUserId(_credStore.VertecCredentials.UserName).Execute(_xmlApiClient);
        long monthSollZeitMinuten = new GetSollZeit(dateStart, dateEnd, ownerId).Execute(_xmlApiClient);
        long monthArbeitszeitMinuten = new GetArbeitszeit(dateStart, dateEnd, ownerId).Execute(_xmlApiClient);
        var monthFerienbezug = TimeSpan.FromMinutes(new GetFerienbezug(dateStart, dateEnd, ownerId).Execute(_xmlApiClient));

        var diffSollActual = TimeSpan.FromMinutes(monthSollZeitMinuten - monthArbeitszeitMinuten);
        var monthSoll = TimeSpan.FromMinutes(monthSollZeitMinuten);
        var monthArbeitszeit = TimeSpan.FromMinutes(monthArbeitszeitMinuten);

        _logger.LogContent($"Summary for {monthName}:");
        var hoursRequired = $" - Hours required:\t{FormatTimeSpan(monthSoll)}";
        var hoursWorked = $" - Hours worked:\t{FormatTimeSpan(monthArbeitszeit)} (work: {FormatTimeSpan(monthArbeitszeit.Subtract(monthFerienbezug))}, holidays: {FormatTimeSpan(monthFerienbezug)})";
        _logger.LogContent(hoursRequired);
        _logger.LogContent(hoursWorked);
        _logger.LogContent($" - {(diffSollActual.TotalMinutes < 0 ? "Overtime:\t\t+" : "Undertime:\t\t-")}{FormatTimeSpan(diffSollActual)}");
        
        if (month != DateTime.Today.Month && DateTime.Today.Day != lastDayMonth)
        {
            return;
        }
        
        // Can do some more fun if current month and there is still time
        var tomorrow = DateTime.Today.AddDays(1);
        var beschaeftigungsGradMinutes = TimeSpan.FromMinutes(new GetBeschaeftigungsgrad(DateTime.Today, ownerId).Execute(_xmlApiClient) * _settings.Elca.DayWorkMinutes);
        
        var remainingSollZeit = TimeSpan.FromMinutes(new GetSollZeit(tomorrow, dateEnd, ownerId).Execute(_xmlApiClient));
        var workedToDate =
            TimeSpan.FromMinutes(new GetArbeitszeit(dateStart, DateTime.Today, ownerId).Execute(_xmlApiClient));
        var plannedHolidays =
            TimeSpan.FromMinutes(new GetFerienbezug(tomorrow, dateEnd, ownerId).Execute(_xmlApiClient));
        var remainingSollZeitAbzFerien = remainingSollZeit
            .Subtract(plannedHolidays);
        var remainingWorkdays = remainingSollZeitAbzFerien.TotalMinutes / beschaeftigungsGradMinutes.TotalMinutes;
        var expectedWorkingHours = TimeSpan.FromMinutes(remainingWorkdays * beschaeftigungsGradMinutes.TotalMinutes);
        var expectedDiff = workedToDate
            .Add(plannedHolidays)
            .Add(expectedWorkingHours)
            .Subtract(monthSoll);

        _logger.LogContent($"\nGetting to zero in {monthName}");
        _logger.LogContent($" - Still need to work:\t\t\t{FormatTimeSpan(remainingSollZeitAbzFerien)} (excl. holidays)");
        _logger.LogContent($" - Remaining workdays:\t\t\t{remainingWorkdays} (excl. holidays)");
        _logger.LogContent($" - Expected hours until end of month:\t{FormatTimeSpan(expectedWorkingHours)} (working {FormatTimeSpan(beschaeftigungsGradMinutes)} per day)");
        _logger.LogContent($" - Expected {(expectedDiff.TotalMinutes > 0 ? "overtime:\t\t\t+" : "undertime\t\t\t-")}{FormatTimeSpan(expectedDiff)}");
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes > 0)
        {
            return $"{(int)Math.Floor(timeSpan.TotalHours)}:{timeSpan.Minutes:00}";
        }
        
        return $"{(int)Math.Floor(Math.Abs(timeSpan.TotalHours))}:{Math.Abs(timeSpan.Minutes):00}";
    }
}