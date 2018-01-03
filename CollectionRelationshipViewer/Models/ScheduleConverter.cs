using System.Management;

namespace CollectionRelationshipViewer.Models
{
    public static class ScheduleConverter
    {
        /// <summary>
        /// This converter takes the value returned from WMI for 
        /// the collection refresh schedule and converts it to something
        /// that is readable.
        /// </summary>
        /// <param name="type">If the type is "1" it has no refresh schedule.</param>
        /// <param name="objs">This is the actual object from WMI. Depending on
        /// the type of schedule it will have different properties available.</param>
        /// <returns>Returns a user readable string corresponding with the refresh schedule.</returns>
        public static string ConvertSchedule(string type, ManagementBaseObject[] objs)
        {
            // There will only be one object in the array, but since
            // it's an array, we need to treat it like one
            foreach(ManagementBaseObject obj in objs)
            {
                // Type 1 means no refresh schedule - otherwise, parse that baby!
                if (type == "1")
                {
                    return "No Refresh Schedule";
                }
                else
                {
                    // __CLASS represents the type of schedule
                    switch (obj["__CLASS"].ToString())
                    {
                        // Weekly recurring schedule.
                        case "SMS_ST_RecurWeekly":
                            return "Occurs every " + obj["ForNumberOfWeeks"].ToString() + " weeks on " + GetDay(obj["Day"].ToString());

                        // Interval recurring
                        case "SMS_ST_RecurInterval":
                            // Every # of days
                            if (obj["DaySpan"].ToString() != "0")
                            {
                                return "Occurs every " + obj["DaySpan"] + " day(s)";
                            }
                            // Every # of hours
                            else if (obj["HourSpan"].ToString() != "0")
                            {
                                return "Occurs every " + obj["HourSpan"] + " hour(s)";
                            }
                            // Ever # of minutes
                            else if (obj["MinuteSpan"].ToString() != "0")
                            {
                                return "Occurs every " + obj["MinuteSpan"] + " minute(s)";
                            }
                            // We should never reach this... but just in case...
                            else
                            {
                                return "WTF?";
                            }

                        // Monthly on date
                        case "SMS_ST_RecurMonthlyByDate":
                            // MonthDay 0 means the last day of every X months
                            if (obj["MonthDay"].ToString() == "0")
                            {
                                return "Occurs on the last day of every " + obj["ForNumberOfMonths"].ToString() + " months";
                            }
                            // MonthDay otherwise, represents a specific day number
                            else
                            {
                                return "Occurs on day " + obj["MonthDay"].ToString() + " of every " + obj["ForNumberOfMonths"].ToString() + " months";
                            }

                        // Monthly on a specific week day (1st Tuesday)
                        case "SMS_ST_RecurMonthlyByWeekday":
                            return "Occurs the " + GetWeek(obj["WeekOrder"].ToString()) + " " + GetDay(obj["Day"].ToString()) + " every " + obj["ForNumberOfMonths"].ToString() + "month(s)";

                        // A single refresh instance.
                        case "SMS_ST_NonRecurring":
                            return "Occurs once on " + ManagementDateTimeConverter.ToDateTime(obj["StartTime"].ToString()).ToLongDateString() + " at " + ManagementDateTimeConverter.ToDateTime(obj["StartTime"].ToString()).ToLongTimeString();

                        // Catchall just in case we can't determine...
                        default:
                            return "Unable to determine...";
                    }
                }
            }
            // Another catchall just in case
            return "";
        }

        // Day number to day name converter
        private static string GetDay(string day)
        {
            switch (day)
            {
                case "1": return "Sunday";
                case "2": return "Monday";
                case "3": return "TuesDay";
                case "4": return "WednesDay";
                case "5": return "ThursDay";
                case "6": return "FriDay";
                case "7": return "Saturday";
                default: return "What?";
            }
        }

        // Month number to month name converter
        private static string GetMonth(string month)
        {
            switch (month)
            {
                case "1": return "January";
                case "2": return "Feburary";
                case "3": return "March";
                case "4": return "April";
                case "5": return "May";
                case "6": return "June";
                case "7": return "July";
                case "8": return "August";
                case "9": return "September";
                case "10": return "October";
                case "11": return "November";
                case "12": return "December";
                default: return "What?";
            }
        }

        // Week number to week name converter
        private static string GetWeek(string week)
        {
            switch(week)
            {
                case "0": return "Last";
                case "1": return "First";
                case "2": return "Second";
                case "3": return "Third";
                case "4": return "Fourth";
                default: return "What??";
            }          
        }
    }
}
