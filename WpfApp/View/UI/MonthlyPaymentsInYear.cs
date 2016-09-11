﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DAL.Model;

namespace WpfApp.View.UI
{
    // оплата за год. имеет массив месяцев (12), в каждом массив оплат
    public class MonthlyPaymentsInYear
    {
        public int Year { get; set; }
        public IList<Month> Months { get; } = new List<Month>();

        public static MonthlyPaymentsInYearsResult ToYears(IQueryable<MonthlyPayment> payments, IQueryable<EnterChild> enters)
        {
            var paymentList = payments.OrderBy(p => p.PaymentDate).ToList();
            var enterList = enters.OrderBy(e => e.EnterDate).ToList();
            var yearsResult = new List<MonthlyPaymentsInYear>();
            
            int paymentIndex = 0;
            int lastYear = -1;
            MonthlyPayment lastPaymentForLastMonth = paymentIndex < paymentList.Count ? paymentList[paymentIndex] : null;

            // for enters
            foreach (var enter in enterList)
            {
                var startDate = enter.EnterDate;
                var endDate = enter.ExpulsionDate ?? DateTime.Now;

                for (int year = startDate.Year; year <= endDate.Year; year++)
                {
                    // concats enters for one year

                    MonthlyPaymentsInYear yearPayment;
                    if (lastYear != year)
                    {
                        yearPayment = new MonthlyPaymentsInYear {Year = year};
                        yearsResult.Add(yearPayment);
                        lastYear = year;
                    }
                    else
                    {
                        yearPayment = yearsResult[yearsResult.Count - 1];
                    }

                    var startMonth = startDate.Year == year ? startDate.Month : 1;
                    var endMonth = endDate.Year == year ? endDate.Month : 12;

                    for (int month = startMonth; month <= endMonth; month++)
                    {
                        var monthPayment = new Month {Index = month-1, Payments = new List<MonthlyPayment>()};

                        // to show archived months. payments = null
                        var months = yearPayment.Months;
                        if (months.Count > 0 && months[months.Count - 1].Index != monthPayment.Index)
                        {
                            for (int i = months[months.Count - 1].Index + 1; i < monthPayment.Index; i++)
                            {
                                var monthlyPayment = paymentList.Count > paymentIndex && paymentList[paymentIndex].PaymentDate.Month == i + 1
                                    ? new List<MonthlyPayment> { paymentList[paymentIndex++] } : null;
                                months.Add(new Month { Index = i, Payments = monthlyPayment, Archived = true });
                            }
                        }

                        // days
                        while (paymentList.Count > paymentIndex)
                        {
                            var payment = paymentList[paymentIndex];
                            var paymentDate = payment.PaymentDate;
                            if (paymentDate.Year != year || paymentDate.Month != month) break;

                            monthPayment.Payments.Add(payment);
                            ++paymentIndex;
                        }
                        if (monthPayment.Payments.Count == 0)
                        {
                            monthPayment.NotPaid = true;
                            var monthlyPayment = new MonthlyPayment
                            {
                                DebtAfterPaying = lastPaymentForLastMonth.DebtAfterPaying + lastPaymentForLastMonth.MoneyPaymentByTarif,
                                MoneyPaymentByTarif = lastPaymentForLastMonth.MoneyPaymentByTarif,
                            };
                            monthPayment.Payments.Add(monthlyPayment);
                            lastPaymentForLastMonth = monthlyPayment;
                        }
                        else
                        {
                            lastPaymentForLastMonth = monthPayment.Payments[monthPayment.Payments.Count-1];
                        }
                        months.Add(monthPayment);
                    }
                }
            }
            var lastPayment = paymentList.Count > 0 ? paymentList[0/*paymentList.Count - 1*/] : null;
            return new MonthlyPaymentsInYearsResult(new ObservableCollection<MonthlyPaymentsInYear>(yearsResult), lastPayment);
        }

        public override string ToString()
        {
            return $"Year = {Year}, Months = {Months}";
        }
    }

    public class MonthlyPaymentsInYearsResult
    {
        public MonthlyPaymentsInYearsResult(ObservableCollection<MonthlyPaymentsInYear> monthlyPaymentsInYears, MonthlyPayment lastPayment)
        {
            MonthlyPaymentsInYears = monthlyPaymentsInYears;
            LastPayment = lastPayment;
        }

        public MonthlyPayment LastPayment { get; }
        public ObservableCollection<MonthlyPaymentsInYear> MonthlyPaymentsInYears { get; } 
    }

    public class MonthCollection
    {
        public IList<Month> Months { get; set; }
    }

    public class Month
    {
        /// <summary>Index of month (0..11)</summary>
        public int Index { get; set; }

        public IList<MonthlyPayment> Payments { get; set; }
        public bool NotPaid { get; set; }
        public bool Archived { get; set; }
    }
}