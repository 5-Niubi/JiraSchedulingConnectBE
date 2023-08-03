using System.Xml.Serialization;

namespace ModelLibrary.DTOs.Export
{
    public class MSPXMLModelDTO
    {
        // using System.Xml.Serialization;
        // XmlSerializer serializer = new XmlSerializer(typeof(Project));
        // using (StringReader reader = new StringReader(xml))
        // {
        //    var test = (Project)serializer.Deserialize(reader);
        // }

        [XmlRoot(ElementName = "View")]
        public class View
        {

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }

            [XmlElement(ElementName = "IsCustomized")]
            public bool IsCustomized
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Views")]
        public class Views
        {

            [XmlElement(ElementName = "View")]
            public List<View> View
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Filter")]
        public class Filter
        {

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Filters")]
        public class Filters
        {

            [XmlElement(ElementName = "Filter")]
            public Filter Filter
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Group")]
        public class Group
        {

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Groups")]
        public class Groups
        {

            [XmlElement(ElementName = "Group")]
            public List<Group> Group
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Table")]
        public class Table
        {

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Tables")]
        public class Tables
        {

            [XmlElement(ElementName = "Table")]
            public Table Table
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "WeekDay")]
        public class WeekDay
        {

            [XmlElement(ElementName = "DayType")]
            public int DayType
            {
                get; set;
            }

            [XmlElement(ElementName = "DayWorking")]
            public int DayWorking
            {
                get; set;
            }

            [XmlElement(ElementName = "WorkingTimes")]
            public WorkingTimes WorkingTimes
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "WorkingTime")]
        public class WorkingTime
        {

            [XmlElement(ElementName = "FromTime")]
            public DateTime FromTime
            {
                get; set;
            }

            [XmlElement(ElementName = "ToTime")]
            public DateTime ToTime
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "WorkingTimes")]
        public class WorkingTimes
        {

            [XmlElement(ElementName = "WorkingTime")]
            public List<WorkingTime> WorkingTime
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "WeekDays")]
        public class WeekDays
        {

            [XmlElement(ElementName = "WeekDay")]
            public List<WeekDay> WeekDay
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Calendar")]
        public class Calendar
        {

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }

            [XmlElement(ElementName = "IsBaseCalendar")]
            public int IsBaseCalendar
            {
                get; set;
            }

            [XmlElement(ElementName = "IsBaselineCalendar")]
            public int IsBaselineCalendar
            {
                get; set;
            }

            [XmlElement(ElementName = "BaseCalendarUID")]
            public int BaseCalendarUID
            {
                get; set;
            }

            [XmlElement(ElementName = "WeekDays")]
            public WeekDays WeekDays
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Calendars")]
        public class Calendars
        {

            [XmlElement(ElementName = "Calendar")]
            public List<Calendar> Calendar
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Task")]
        public class Task
        {

            [XmlElement(ElementName = "PredecessorLink")]
            public List<PredecessorLink> PredecessorLink
            {
                get; set;
            }

            [XmlElement(ElementName = "IsPublished")]
            public int IsPublished
            {
                get; set;
            }

            [XmlElement(ElementName = "CommitmentType")]
            public int CommitmentType
            {
                get; set;
            }

            [XmlElement(ElementName = "BoardStatusColumnOrderingID")]
            public int BoardStatusColumnOrderingID
            {
                get; set;
            }

            [XmlElement(ElementName = "SprintColumnOrderingID")]
            public int SprintColumnOrderingID
            {
                get; set;
            }

            [XmlElement(ElementName = "NextAvailableBoardStatusColumnOrderingID")]
            public int NextAvailableBoardStatusColumnOrderingID
            {
                get; set;
            }

            [XmlElement(ElementName = "NextAvailableSprintOrderingID")]
            public int NextAvailableSprintOrderingID
            {
                get; set;
            }

            [XmlElement(ElementName = "SprintUID")]
            public int SprintUID
            {
                get; set;
            }

            [XmlElement(ElementName = "BoardColumnUID")]
            public int BoardColumnUID
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "PredecessorLink")]
        public class PredecessorLink
        {

            [XmlElement(ElementName = "PredecessorUID")]
            public int PredecessorUID
            {
                get; set;
            }

            [XmlElement(ElementName = "Type")]
            public int Type
            {
                get; set;
            }

            [XmlElement(ElementName = "CrossProject")]
            public int CrossProject
            {
                get; set;
            }

            [XmlElement(ElementName = "LinkLag")]
            public int LinkLag
            {
                get; set;
            }

            [XmlElement(ElementName = "LagFormat")]
            public int LagFormat
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Tasks")]
        public class Tasks
        {

            [XmlElement(ElementName = "Task")]
            public List<Task> Task
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Resource")]
        public class Resource
        {

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "ID")]
            public int ID
            {
                get; set;
            }

            [XmlElement(ElementName = "Type")]
            public int Type
            {
                get; set;
            }

            [XmlElement(ElementName = "IsNull")]
            public int IsNull
            {
                get; set;
            }

            [XmlElement(ElementName = "MaxUnits")]
            public double MaxUnits
            {
                get; set;
            }

            [XmlElement(ElementName = "PeakUnits")]
            public double PeakUnits
            {
                get; set;
            }

            [XmlElement(ElementName = "OverAllocated")]
            public int OverAllocated
            {
                get; set;
            }

            [XmlElement(ElementName = "CanLevel")]
            public int CanLevel
            {
                get; set;
            }

            [XmlElement(ElementName = "AccrueAt")]
            public int AccrueAt
            {
                get; set;
            }

            [XmlElement(ElementName = "Work")]
            public string Work
            {
                get; set;
            }

            [XmlElement(ElementName = "RegularWork")]
            public string RegularWork
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeWork")]
            public string OvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualWork")]
            public string ActualWork
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingWork")]
            public string RemainingWork
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualOvertimeWork")]
            public string ActualOvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingOvertimeWork")]
            public string RemainingOvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "PercentWorkComplete")]
            public int PercentWorkComplete
            {
                get; set;
            }

            [XmlElement(ElementName = "StandardRate")]
            public int StandardRate
            {
                get; set;
            }

            [XmlElement(ElementName = "StandardRateFormat")]
            public int StandardRateFormat
            {
                get; set;
            }

            [XmlElement(ElementName = "Cost")]
            public int Cost
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeRate")]
            public int OvertimeRate
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeRateFormat")]
            public int OvertimeRateFormat
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeCost")]
            public int OvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "CostPerUse")]
            public int CostPerUse
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualCost")]
            public int ActualCost
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualOvertimeCost")]
            public int ActualOvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingCost")]
            public int RemainingCost
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingOvertimeCost")]
            public int RemainingOvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "WorkVariance")]
            public double WorkVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "CostVariance")]
            public int CostVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "SV")]
            public double SV
            {
                get; set;
            }

            [XmlElement(ElementName = "CV")]
            public double CV
            {
                get; set;
            }

            [XmlElement(ElementName = "ACWP")]
            public double ACWP
            {
                get; set;
            }

            [XmlElement(ElementName = "CalendarUID")]
            public int CalendarUID
            {
                get; set;
            }

            [XmlElement(ElementName = "BCWS")]
            public double BCWS
            {
                get; set;
            }

            [XmlElement(ElementName = "BCWP")]
            public double BCWP
            {
                get; set;
            }

            [XmlElement(ElementName = "IsGeneric")]
            public int IsGeneric
            {
                get; set;
            }

            [XmlElement(ElementName = "IsInactive")]
            public int IsInactive
            {
                get; set;
            }

            [XmlElement(ElementName = "IsEnterprise")]
            public int IsEnterprise
            {
                get; set;
            }

            [XmlElement(ElementName = "BookingType")]
            public int BookingType
            {
                get; set;
            }

            [XmlElement(ElementName = "CreationDate")]
            public DateTime CreationDate
            {
                get; set;
            }

            [XmlElement(ElementName = "IsCostResource")]
            public int IsCostResource
            {
                get; set;
            }

            [XmlElement(ElementName = "IsBudget")]
            public int IsBudget
            {
                get; set;
            }

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }

            [XmlElement(ElementName = "Initials")]
            public string Initials
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Resources")]
        public class Resources
        {

            [XmlElement(ElementName = "Resource")]
            public List<Resource> Resource
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "TimephasedData")]
        public class TimephasedData
        {

            [XmlElement(ElementName = "Type")]
            public int Type
            {
                get; set;
            }

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "Start")]
            public DateTime Start
            {
                get; set;
            }

            [XmlElement(ElementName = "Finish")]
            public DateTime Finish
            {
                get; set;
            }

            [XmlElement(ElementName = "Unit")]
            public int Unit
            {
                get; set;
            }

            [XmlElement(ElementName = "Value")]
            public string Value
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Assignment")]
        public class Assignment
        {

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "TaskUID")]
            public int TaskUID
            {
                get; set;
            }

            [XmlElement(ElementName = "ResourceUID")]
            public int ResourceUID
            {
                get; set;
            }

            [XmlElement(ElementName = "PercentWorkComplete")]
            public int PercentWorkComplete
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualCost")]
            public int ActualCost
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualOvertimeCost")]
            public int ActualOvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualOvertimeWork")]
            public string ActualOvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualWork")]
            public string ActualWork
            {
                get; set;
            }

            [XmlElement(ElementName = "ACWP")]
            public double ACWP
            {
                get; set;
            }

            [XmlElement(ElementName = "Confirmed")]
            public int Confirmed
            {
                get; set;
            }

            [XmlElement(ElementName = "Cost")]
            public int Cost
            {
                get; set;
            }

            [XmlElement(ElementName = "CostRateTable")]
            public int CostRateTable
            {
                get; set;
            }

            [XmlElement(ElementName = "RateScale")]
            public int RateScale
            {
                get; set;
            }

            [XmlElement(ElementName = "CostVariance")]
            public int CostVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "CV")]
            public double CV
            {
                get; set;
            }

            [XmlElement(ElementName = "Delay")]
            public int Delay
            {
                get; set;
            }

            [XmlElement(ElementName = "Finish")]
            public DateTime Finish
            {
                get; set;
            }

            [XmlElement(ElementName = "FinishVariance")]
            public int FinishVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "WorkVariance")]
            public double WorkVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "HasFixedRateUnits")]
            public int HasFixedRateUnits
            {
                get; set;
            }

            [XmlElement(ElementName = "FixedMaterial")]
            public int FixedMaterial
            {
                get; set;
            }

            [XmlElement(ElementName = "LevelingDelay")]
            public int LevelingDelay
            {
                get; set;
            }

            [XmlElement(ElementName = "LevelingDelayFormat")]
            public int LevelingDelayFormat
            {
                get; set;
            }

            [XmlElement(ElementName = "LinkedFields")]
            public int LinkedFields
            {
                get; set;
            }

            [XmlElement(ElementName = "Milestone")]
            public int Milestone
            {
                get; set;
            }

            [XmlElement(ElementName = "Overallocated")]
            public int Overallocated
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeCost")]
            public int OvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "OvertimeWork")]
            public string OvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "RegularWork")]
            public string RegularWork
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingCost")]
            public int RemainingCost
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingOvertimeCost")]
            public int RemainingOvertimeCost
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingOvertimeWork")]
            public string RemainingOvertimeWork
            {
                get; set;
            }

            [XmlElement(ElementName = "RemainingWork")]
            public string RemainingWork
            {
                get; set;
            }

            [XmlElement(ElementName = "ResponsePending")]
            public int ResponsePending
            {
                get; set;
            }

            [XmlElement(ElementName = "Start")]
            public DateTime Start
            {
                get; set;
            }

            [XmlElement(ElementName = "StartVariance")]
            public int StartVariance
            {
                get; set;
            }

            [XmlElement(ElementName = "Units")]
            public int Units
            {
                get; set;
            }

            [XmlElement(ElementName = "UpdateNeeded")]
            public int UpdateNeeded
            {
                get; set;
            }

            [XmlElement(ElementName = "VAC")]
            public double VAC
            {
                get; set;
            }

            [XmlElement(ElementName = "Work")]
            public string Work
            {
                get; set;
            }

            [XmlElement(ElementName = "WorkContour")]
            public int WorkContour
            {
                get; set;
            }

            [XmlElement(ElementName = "BCWS")]
            public double BCWS
            {
                get; set;
            }

            [XmlElement(ElementName = "BCWP")]
            public double BCWP
            {
                get; set;
            }

            [XmlElement(ElementName = "BookingType")]
            public int BookingType
            {
                get; set;
            }

            [XmlElement(ElementName = "CreationDate")]
            public DateTime CreationDate
            {
                get; set;
            }

            [XmlElement(ElementName = "BudgetCost")]
            public int BudgetCost
            {
                get; set;
            }

            [XmlElement(ElementName = "BudgetWork")]
            public string BudgetWork
            {
                get; set;
            }

            [XmlElement(ElementName = "TimephasedData")]
            public List<TimephasedData> TimephasedData
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Assignments")]
        public class Assignments
        {

            [XmlElement(ElementName = "Assignment")]
            public List<Assignment> Assignment
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "BoardColumn")]
        public class BoardColumn
        {

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "ID")]
            public int ID
            {
                get; set;
            }

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "BoardColumns")]
        public class BoardColumns
        {

            [XmlElement(ElementName = "BoardColumn")]
            public List<BoardColumn> BoardColumn
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Sprint")]
        public class Sprint
        {

            [XmlElement(ElementName = "UID")]
            public int UID
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "ID")]
            public int ID
            {
                get; set;
            }

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }

            [XmlElement(ElementName = "DurationUnits")]
            public int DurationUnits
            {
                get; set;
            }

            [XmlElement(ElementName = "Duration")]
            public string Duration
            {
                get; set;
            }

            [XmlElement(ElementName = "Start")]
            public DateTime Start
            {
                get; set;
            }

            [XmlElement(ElementName = "Finish")]
            public DateTime Finish
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Sprints")]
        public class Sprints
        {

            [XmlElement(ElementName = "Sprint")]
            public List<Sprint> Sprint
            {
                get; set;
            }
        }

        [XmlRoot(ElementName = "Project")]
        public class Project
        {

            [XmlElement(ElementName = "SaveVersion")]
            public int SaveVersion
            {
                get; set;
            }

            [XmlElement(ElementName = "BuildNumber")]
            public string BuildNumber
            {
                get; set;
            }

            [XmlElement(ElementName = "Name")]
            public string Name
            {
                get; set;
            }

            [XmlElement(ElementName = "GUID")]
            public string GUID
            {
                get; set;
            }

            [XmlElement(ElementName = "Title")]
            public string Title
            {
                get; set;
            }

            [XmlElement(ElementName = "CreationDate")]
            public DateTime CreationDate
            {
                get; set;
            }

            [XmlElement(ElementName = "LastSaved")]
            public DateTime LastSaved
            {
                get; set;
            }

            [XmlElement(ElementName = "ScheduleFromStart")]
            public int ScheduleFromStart
            {
                get; set;
            }

            [XmlElement(ElementName = "StartDate")]
            public DateTime StartDate
            {
                get; set;
            }

            [XmlElement(ElementName = "FinishDate")]
            public DateTime FinishDate
            {
                get; set;
            }

            [XmlElement(ElementName = "FYStartDate")]
            public int FYStartDate
            {
                get; set;
            }

            [XmlElement(ElementName = "CriticalSlackLimit")]
            public int CriticalSlackLimit
            {
                get; set;
            }

            [XmlElement(ElementName = "CurrencyDigits")]
            public int CurrencyDigits
            {
                get; set;
            }

            [XmlElement(ElementName = "CurrencySymbol")]
            public string CurrencySymbol
            {
                get; set;
            }

            [XmlElement(ElementName = "CurrencyCode")]
            public string CurrencyCode
            {
                get; set;
            }

            [XmlElement(ElementName = "CurrencySymbolPosition")]
            public int CurrencySymbolPosition
            {
                get; set;
            }

            [XmlElement(ElementName = "CalendarUID")]
            public int CalendarUID
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultStartTime")]
            public DateTime DefaultStartTime
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultFinishTime")]
            public DateTime DefaultFinishTime
            {
                get; set;
            }

            [XmlElement(ElementName = "MinutesPerDay")]
            public int MinutesPerDay
            {
                get; set;
            }

            [XmlElement(ElementName = "MinutesPerWeek")]
            public int MinutesPerWeek
            {
                get; set;
            }

            [XmlElement(ElementName = "DaysPerMonth")]
            public int DaysPerMonth
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultTaskType")]
            public int DefaultTaskType
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultFixedCostAccrual")]
            public int DefaultFixedCostAccrual
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultStandardRate")]
            public int DefaultStandardRate
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultOvertimeRate")]
            public int DefaultOvertimeRate
            {
                get; set;
            }

            [XmlElement(ElementName = "DurationFormat")]
            public int DurationFormat
            {
                get; set;
            }

            [XmlElement(ElementName = "WorkFormat")]
            public int WorkFormat
            {
                get; set;
            }

            [XmlElement(ElementName = "EditableActualCosts")]
            public int EditableActualCosts
            {
                get; set;
            }

            [XmlElement(ElementName = "HonorConstraints")]
            public int HonorConstraints
            {
                get; set;
            }

            [XmlElement(ElementName = "InsertedProjectsLikeSummary")]
            public int InsertedProjectsLikeSummary
            {
                get; set;
            }

            [XmlElement(ElementName = "MultipleCriticalPaths")]
            public int MultipleCriticalPaths
            {
                get; set;
            }

            [XmlElement(ElementName = "NewTasksEffortDriven")]
            public int NewTasksEffortDriven
            {
                get; set;
            }

            [XmlElement(ElementName = "NewTasksEstimated")]
            public int NewTasksEstimated
            {
                get; set;
            }

            [XmlElement(ElementName = "SplitsInProgressTasks")]
            public int SplitsInProgressTasks
            {
                get; set;
            }

            [XmlElement(ElementName = "SpreadActualCost")]
            public int SpreadActualCost
            {
                get; set;
            }

            [XmlElement(ElementName = "SpreadPercentComplete")]
            public int SpreadPercentComplete
            {
                get; set;
            }

            [XmlElement(ElementName = "TaskUpdatesResource")]
            public int TaskUpdatesResource
            {
                get; set;
            }

            [XmlElement(ElementName = "FiscalYearStart")]
            public int FiscalYearStart
            {
                get; set;
            }

            [XmlElement(ElementName = "WeekStartDay")]
            public int WeekStartDay
            {
                get; set;
            }

            [XmlElement(ElementName = "MoveCompletedEndsBack")]
            public int MoveCompletedEndsBack
            {
                get; set;
            }

            [XmlElement(ElementName = "MoveRemainingStartsBack")]
            public int MoveRemainingStartsBack
            {
                get; set;
            }

            [XmlElement(ElementName = "MoveRemainingStartsForward")]
            public int MoveRemainingStartsForward
            {
                get; set;
            }

            [XmlElement(ElementName = "MoveCompletedEndsForward")]
            public int MoveCompletedEndsForward
            {
                get; set;
            }

            [XmlElement(ElementName = "BaselineForEarnedValue")]
            public int BaselineForEarnedValue
            {
                get; set;
            }

            [XmlElement(ElementName = "AutoAddNewResourcesAndTasks")]
            public int AutoAddNewResourcesAndTasks
            {
                get; set;
            }

            [XmlElement(ElementName = "CurrentDate")]
            public DateTime CurrentDate
            {
                get; set;
            }

            [XmlElement(ElementName = "MicrosoftProjectServerURL")]
            public int MicrosoftProjectServerURL
            {
                get; set;
            }

            [XmlElement(ElementName = "Autolink")]
            public int Autolink
            {
                get; set;
            }

            [XmlElement(ElementName = "NewTaskStartDate")]
            public int NewTaskStartDate
            {
                get; set;
            }

            [XmlElement(ElementName = "NewTasksAreManual")]
            public int NewTasksAreManual
            {
                get; set;
            }

            [XmlElement(ElementName = "DefaultTaskEVMethod")]
            public int DefaultTaskEVMethod
            {
                get; set;
            }

            [XmlElement(ElementName = "ProjectExternallyEdited")]
            public int ProjectExternallyEdited
            {
                get; set;
            }

            [XmlElement(ElementName = "ExtendedCreationDate")]
            public DateTime ExtendedCreationDate
            {
                get; set;
            }

            [XmlElement(ElementName = "ActualsInSync")]
            public int ActualsInSync
            {
                get; set;
            }

            [XmlElement(ElementName = "RemoveFileProperties")]
            public int RemoveFileProperties
            {
                get; set;
            }

            [XmlElement(ElementName = "AdminProject")]
            public int AdminProject
            {
                get; set;
            }

            [XmlElement(ElementName = "UpdateManuallyScheduledTasksWhenEditingLinks")]
            public int UpdateManuallyScheduledTasksWhenEditingLinks
            {
                get; set;
            }

            [XmlElement(ElementName = "KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled")]
            public int KeepTaskOnNearestWorkingTimeWhenMadeAutoScheduled
            {
                get; set;
            }

            [XmlElement(ElementName = "SprintLength")]
            public int SprintLength
            {
                get; set;
            }

            [XmlElement(ElementName = "AgileMode")]
            public int AgileMode
            {
                get; set;
            }

            [XmlElement(ElementName = "Views")]
            public Views Views
            {
                get; set;
            }

            [XmlElement(ElementName = "Filters")]
            public Filters Filters
            {
                get; set;
            }

            [XmlElement(ElementName = "Groups")]
            public Groups Groups
            {
                get; set;
            }

            [XmlElement(ElementName = "Tables")]
            public Tables Tables
            {
                get; set;
            }

            [XmlElement(ElementName = "Maps")]
            public object Maps
            {
                get; set;
            }

            [XmlElement(ElementName = "Reports")]
            public object Reports
            {
                get; set;
            }

            [XmlElement(ElementName = "Drawings")]
            public object Drawings
            {
                get; set;
            }

            [XmlElement(ElementName = "DataLinks")]
            public object DataLinks
            {
                get; set;
            }

            [XmlElement(ElementName = "VBAProjects")]
            public object VBAProjects
            {
                get; set;
            }

            [XmlElement(ElementName = "OutlineCodes")]
            public object OutlineCodes
            {
                get; set;
            }

            [XmlElement(ElementName = "WBSMasks")]
            public object WBSMasks
            {
                get; set;
            }

            [XmlElement(ElementName = "ExtendedAttributes")]
            public object ExtendedAttributes
            {
                get; set;
            }

            [XmlElement(ElementName = "Calendars")]
            public Calendars Calendars
            {
                get; set;
            }

            [XmlElement(ElementName = "Tasks")]
            public Tasks Tasks
            {
                get; set;
            }

            [XmlElement(ElementName = "Resources")]
            public Resources Resources
            {
                get; set;
            }

            [XmlElement(ElementName = "Assignments")]
            public Assignments Assignments
            {
                get; set;
            }

            [XmlElement(ElementName = "BoardColumns")]
            public BoardColumns BoardColumns
            {
                get; set;
            }

            [XmlElement(ElementName = "Sprints")]
            public Sprints Sprints
            {
                get; set;
            }

            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns
            {
                get; set;
            }

            [XmlText]
            public string Text
            {
                get; set;
            }
        }


    }
}
