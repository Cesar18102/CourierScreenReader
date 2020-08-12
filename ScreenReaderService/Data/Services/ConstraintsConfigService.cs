using System;
using System.IO;
using System.Linq;

using Autofac;

using ScreenReaderService.Services;
using ScreenReaderService.Data.Exceptions;

namespace ScreenReaderService.Data.Services
{
    public class ConstraintsConfigService : ObjectFileMappingService<ConstraintsInfo>
    {
        private const string CONSTRAINTS_FILE_NAME = "constraints.json";

        private static readonly string CONSTRAINTS_CONFIG_PATH = Path.Combine(
            Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public ConstraintsConfigService() : base(CONSTRAINTS_CONFIG_PATH, CONSTRAINTS_FILE_NAME) { }

        public ConstraintsInfo Constraints
        {
            get => Data;
            set => Data = value;
        }

        private WorkService WorkService = DependencyHolder.Dependencies.Resolve<WorkService>();

        public void CheckInvariantConstraintsPassed()
        {
            if (WorkService.WorkInfo.ActiveOrders.Count == Constraints.MaxActiveOrdersAtTime)
                throw new ConstraintNotPassedException("active orders count exceeded");

            int todayTakenOrdersCount = WorkService.WorkInfo.ActiveOrders.Count + WorkService.WorkInfo.DoneOrders.Count;

            if (todayTakenOrdersCount == Constraints.MaxOrdersPerDay)
                throw new ConstraintNotPassedException("daily orders count exceeded");
        }

        public void PreOpenCheckOrderPassesConstraints(Order order)
        {
            if (!IsTimePeriodsIntersect(order.MinTakeTime, order.MaxTakeTime, Constraints.MinTakeTime, Constraints.MaxTakeTime))
                throw new ConstraintNotPassedException("take time is out of bounds");

            if (!IsTimePeriodsIntersect(order.MinDeliverTime, order.MaxDeliverTime, Constraints.MinDeliverTime, Constraints.MaxDeliverTime))
                throw new ConstraintNotPassedException("deliver time is out of bounds");
        }

        public void PostOpenCheckOrderPassesConstraints(Order order)
        {
            bool orderPriceGreaterThanMaxOrderPrice = order.BuyPrice >= Constraints.MaxOrderPrice;

            int moneyInWork = WorkService.WorkInfo.ActiveOrders.Sum(order => order.BuyPrice);
            bool orderPriceExceedsMaxTotalSpend = moneyInWork + order.BuyPrice >= Constraints.MaxTotalSpent;

            if (orderPriceGreaterThanMaxOrderPrice || orderPriceExceedsMaxTotalSpend)
                throw new ConstraintNotPassedException("buy price exceeded");
        }

        private bool IsTimePeriodsIntersect(DateTime? min, DateTime? max, DateTime? minOther, DateTime? maxOther)
        {
            return IsTimePointInsidePeriod(min, max, minOther) || IsTimePointInsidePeriod(min, max, maxOther);
        }

        private bool IsTimePointInsidePeriod(DateTime? min, DateTime? max, DateTime? point)
        {
            if (!point.HasValue)
                return true;

            if (!min.HasValue && !max.HasValue)
                return true;

            if (!min.HasValue)
                return point.Value <= max.Value;

            if (!max.HasValue)
                return point.Value >= min.Value;

            return point >= min && point <= max;
        }
    }
}
