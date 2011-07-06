using System;

namespace IronJS.Runtime.Objects
{
    public class DateObject : CommonObject
    {
        public DateObject(Environment env, DateTime date)
            : base(env, env.Maps.Base, env.Prototypes.Date)
        {
            this.Date = date;
        }

        public DateObject(Environment env, long ticks)
            : this(env, TicksToDateTime(ticks))
        {
        }

        public DateObject(Environment env, double ticks)
            : this(env, TicksToDateTime(ticks))
        {
        }

        private static long offset = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
        private static long tickScale = 10000L;

        public override string ClassName
        {
            get { return "Date"; }
        }

        public DateTime Date { get; set; }

        public bool HasValidDate
        {
            get { return this.Date != DateTime.MinValue; }
        }

        public override BoxedValue DefaultValue(DefaultValueHint hint)
        {
            if (hint == DefaultValueHint.None)
            {
                hint = DefaultValueHint.String;
            }

            return base.DefaultValue(hint);
        }

        public static DateTime TicksToDateTime(long ticks)
        {
            return new DateTime(ticks * tickScale + offset, DateTimeKind.Utc);
        }

        public static DateTime TicksToDateTime(double ticks)
        {
            if (double.IsNaN(ticks) || double.IsInfinity(ticks))
            {
                return DateTime.MinValue;
            }

            return TicksToDateTime((long)ticks);
        }

        public static long DateTimeToTicks(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - offset) / tickScale;
        }
    }
}
