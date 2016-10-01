using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.DataAccessLayer.Enums
{
    public enum NotificationsTypes
    {
        PaymentDue = 1,
        DepositDue = 2,
        TicketingComplete = 3,
        TicketingError = 4,
        PNRChangedByTicketingDesk = 5,
        PNRCanceled = 6,
        PNRReplaced = 7,
        TicketingLate = 8,
        MessagebyAirline = 9,
        NoResponsetoSpecialService = 10,
        [Obsolete]  
        PNRChangedByAirline = 11,   // These notifications are being replaced by new ScheduleChange notifications
        SpecialInstructions = 12,
        AgentDelegations = 13, //cc for agent delegations
        ScheduleChanges = 14,
        NoLastTicketingDate = 15,
        AgentGeneratedNotification = 16,
        CruiseCancelled = 17,
        PlanningFeeRefunded = 18,
        BookingCancelledByAirline = 19,
        LastTicketingDatePassed = 20
    }
}
