﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DAL.Model
{
    public abstract class BaseEntity<TId> : IDataErrorInfo
    {
        [Key]
        public TId Id { get; set; }

        public string this[string columnName]
        {
            get
            {
                string error = GetErrorInternal(columnName);
                Error = error;
                return error;
            }
        }

        protected virtual string GetErrorInternal(string columnName) => null;

        [NotMapped]
        public string Error { get; protected set; }

#if DEBUG
        // ReSharper disable once StaticMemberInGenericType
        /// <summary>Uniqueness keeper (for debug)</summary>
        public static int UidKeeper = 0;
        /// <summary>Unique identifier of entity (for debug)</summary>
        public int Uid { get; } = ++UidKeeper;
#endif
    }

    public abstract class DateTimeNowAsDefaultEntity : BaseEntity<int>
    {
        private DateTime? _dateTime;
        /// <summary>Provides lazy DateTime.Now</summary>
        /// <example>
        /*
         to use:
         
        {
            get { return Date; }
            set { Date = value; }
        }

         */
        /// </example>
        protected DateTime Date
        {
            get
            {
                if (_dateTime == null)
                {
                    _dateTime = DateTime.Now;
                }
                return _dateTime.Value;
            }
            set
            {
                _dateTime = value;
            }
        }

    }

    public class Group : DateTimeNowAsDefaultEntity
    {
        [NotMapped]
        public int ChildCount { get; set; }

        [Required, MaxLength(64)]
        public string Name { get; set; }

        public Groups GroupType { get; set; }

        public DateTime CreatedDate
        {
            get { return Date; }
            set { Date = value; }
        }

        public virtual ICollection<Child> Children { get; set; }

        protected override string GetErrorInternal(string columnName)
        {
            switch (columnName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Wrong Name";
                    break;
                case nameof(GroupType):
                    if (!Enum.IsDefined(typeof (Groups), GroupType))
                        return nameof(GroupType) + " hasn't chosen";
                    break;
            }
            return null;
        }

        public bool IsValid()
        {
            return
                GetErrorInternal(nameof(Name)) == null &&
                GetErrorInternal(nameof(GroupType)) == null;
        }
    }

    [Flags]
    public enum Groups : byte
    {
        // pair with all
        Finished = 1,

        // pair with Finished
        Nursery = 2,
        Junior1 = 4,
        Junior2 = 6,
        Middle = 8,
        Older = 10,
        Preparatory = 12,
    }

    public class Person : BaseEntity<int>
    {
        [Required, MaxLength(64)]
        public string FirstName { get; set; }
        [Required, MaxLength(64)]
        public string LastName { get; set; }
        [MaxLength(64)]
        public string Patronymic { get; set; }

        [NotMapped]
        public string FullName => string.Join(" ", LastName, FirstName, Patronymic);

        [MaxLength(255)]
        public string PhotoPath { get; set; }

        protected override string GetErrorInternal(string columnName)
        {
            switch (columnName)
            {
                case nameof(FirstName):
                    if (string.IsNullOrWhiteSpace(FirstName))
                        return "Firstname is not valid";
                    break;
                case nameof(LastName):
                    if (string.IsNullOrWhiteSpace(LastName))
                        return "LastName is not valid";
                    break;
                case nameof(Patronymic):
                    if (string.IsNullOrWhiteSpace(Patronymic))
                        return "Patronymic is not valid";
                    break;
                case nameof(PhotoPath):
                    if (!string.IsNullOrWhiteSpace(PhotoPath) && PhotoPath.Length > 255)
                        return "PhotoPath is too long";
                    break;
            }
            return null;
        }

        public bool IsValid()
        {
            return
                GetErrorInternal(nameof(FirstName)) == null &&
                GetErrorInternal(nameof(LastName)) == null &&
                GetErrorInternal(nameof(Patronymic)) == null &&
                GetErrorInternal(nameof(PhotoPath)) == null;
        }

        public override string ToString()
        {
            return $"{nameof(Id)} = {Id} {nameof(FirstName)} = {FirstName}, {nameof(LastName)} = {LastName}, {nameof(Patronymic)} = {Patronymic}.";
        }
    }

    public class Parent : BaseEntity<int>
    {
        public virtual Person Person { get; set; }

        [Required, MaxLength(128)]
        public string LocationAddress { get; set; }
        [MaxLength(128)]
        public string ResidenceAddress { get; set; }
        [MaxLength(128)]
        public string WorkAddress { get; set; }

        [Required, MaxLength(10), MinLength(10)]
        public string PassportSeries { get; set; }
        [Required, MaxLength(256)]
        public string PassportIssuedBy { get; set; }
        public DateTime PassportIssueDate { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public virtual ICollection<ParentChild> ParentsChildren { get; set; }

        protected override string GetErrorInternal(string columnName)
        {
            switch (columnName)
            {
                case nameof(PassportSeries):
                    if (PassportSeries.Length != 10)
                        return "Серия и номер паспорта должны состоять из 10 цифр";
                    if (!PassportSeries.All(char.IsDigit))
                        return "Серия и номер паспорта должны состоять только из цифр";
                    break;
            }
            return null;
        }

        public bool IsValid()
        {
            return GetErrorInternal(nameof(PassportSeries)) == null;
        }
    }

    public class Child : DateTimeNowAsDefaultEntity
    {
        public Person Person { get; set; }

        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        [Required, MaxLength(128)]
        public string LocationAddress { get; set; }

        private DateTime _birthDate;

        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value.Date; }
        }

        public Sex Sex { get; set; }

        [NotMapped]
        public EnterChild LastEnterChild { get; set; }
        [NotMapped]
        public MonthlyPayment LastMonthlyPayment { get; set; }
        [NotMapped]
        public double? MonthlyDebt { get; set; }

        public bool IsNobody { get; set; }

        public int TarifId { get; set; }

        public virtual Tarif Tarif { get; set; }

        public virtual ICollection<ParentChild> ParentsChildren { get; set; }
        public virtual ICollection<MonthlyPayment> Payments { get; set; }

        public virtual ICollection<EnterChild> EnterChildren { get; set; }

        protected override string GetErrorInternal(string columnName)
        {
            switch (columnName)
            {
                case nameof(LocationAddress):
                    if (string.IsNullOrWhiteSpace(LocationAddress))
                        return "Is null or spaces";
                    break;
                case nameof(Group):
                    if (Group == null && GroupId == 0)
                        return "Group must be chosen";
                    break;
                case nameof(Person):
                    if (Person == null)
                        return "Person cannot be null";
                    break;
                case nameof(Tarif):
                    if (Tarif == null && TarifId <= 0)
                        return "Tarif is required";
                    break;
            }
            return null;
        }

        public bool IsValid()
        {
            return
                GetErrorInternal(nameof(LocationAddress)) == null &&
                GetErrorInternal(nameof(Group)) == null &&
                GetErrorInternal(nameof(Tarif)) == null &&
                Person.IsValid();
        }

        public override string ToString()
        {
            return "Child: " + Person;
        }
    }

    [Table("ParentChild")]
    public class ParentChild
    {
        [Key, Column(Order = 0)]
        public int ChildId { get; set; }
        [Key, Column(Order = 1)]
        public int ParentId { get; set; }

        public /*virtual*/ Child Child { get; set; }
        public /*virtual*/ Parent Parent { get; set; }

        public Parents ParentType { get; set; }
        [MaxLength(32)]
        public string ParentTypeText { get; set; }
    }

    public class Tarif : BaseEntity<int>, ICloneable
    {
        public double MonthlyPayment { get; set; }
        public double AnnualPayment { get; set; }
        [Required, MaxLength(255), MinLength(3)]
        public string Note { get; set; }

        [NotMapped]
        public int ChildCount { get; set; }
        public virtual ICollection<Child> Children { get; set; }


        protected override string GetErrorInternal(string columnName)
        {
            string err = null;
            switch (columnName)
            {
                case nameof(MonthlyPayment):
                    if (MonthlyPayment < 0)
                        err = "Месячная оплата не может быть ниже нуля";
                    break;
                case nameof(AnnualPayment):
                    if (AnnualPayment < 0)
                        err = "Годовая оплата не может быть ниже нуля";
                    break;
                case nameof(Note):
                    if (string.IsNullOrWhiteSpace(Note))
                        err = "Примечание не должно быть пустой строкой или состоять из пробелов";
                    else if (Note.Length < 3)
                        err = "Минимум 3 символа";
                    break;
            }
            return err;
        }

        public bool IsValid()
        {
            return
                GetErrorInternal(nameof(MonthlyPayment)) == null &&
                GetErrorInternal(nameof(AnnualPayment)) == null &&
                GetErrorInternal(nameof(Note)) == null;
        }

        public override string ToString()
        {
            return $"{nameof(Id)} = {Id}, {nameof(Note)} = {Note}";
        }

        object ICloneable.Clone() => Clone();

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public Tarif Clone(bool full = false)
        {
            var clone = new Tarif
            {
                Id = Id,
                AnnualPayment = AnnualPayment,
                ChildCount = ChildCount,
                MonthlyPayment = MonthlyPayment,
                Note = Note,
            };
            if (full) clone.ChildCount = ChildCount;
            return clone;
        }
    }

    public abstract class SimplePayment : DateTimeNowAsDefaultEntity
    {
        public int ChildId { get; set; }
        public Child Child { get; set; }

        public DateTime PaymentDate
        {
            get { return Date; }
            set { Date = value; }
        }

        public double MoneyPaymentByTarif { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
    }

    public class MonthlyPayment : SimplePayment
    {
        public int? PayDayCount { get; set; }
        public int? MonthDayCount { get; set; }

        public double PaidMoney { get; set; }
        public double DebtAfterPaying { get; set; }


        // for the views
        public double Credit => Math.Max(DebtAfterPaying, 0);
        public double Debit => Math.Max(-DebtAfterPaying, 0);
    }

    // annual payment
    public class RangePayment : SimplePayment
    {
        public DateTime PaymentFrom { get; set; }
        public DateTime PaymentTo { get; set; }
    }

    [Table("EnterChildHistory")]
    public class EnterChild : DateTimeNowAsDefaultEntity
    {
        [MaxLength(64)]
        public string ExpulsionNote { get; set; }

        public int ChildId { get; set; }
        public virtual Child Child { get; set; }

        public DateTime EnterDate
        {
            get { return Date; }
            set { Date = value; }
        }

        public DateTime? ExpulsionDate { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(Id)} = {Id}, " +
                $"{nameof(EnterDate)} = {EnterDate}, " +
                $"{nameof(ExpulsionDate)} = {ExpulsionDate?.ToString() ?? "NULL"}";
        }
    }

    public class Expense : DateTimeNowAsDefaultEntity
    {
        public ExpenseType ExpenseType { get; set; }
        public double Money { get; set; }

        public DateTime ExpenseDate
        {
            get { return Date; }
            set { Date = value; }
        }

        [MaxLength(255)]
        public string Description { get; set; }
    }

    public class Income : DateTimeNowAsDefaultEntity, ICloneable
    {
        [NotMapped]
        public string Prefix { get; set; }

        [MaxLength(64)]
        public string PersonName { get; set; }
        public double Money { get; set; }
        [MaxLength(255)]
        public string Note { get; set; }

        public DateTime IncomeDate
        {
            get { return Date; }
            set { Date = value; }
        }

        public bool IsValid()
        {
            return
                GetErrorInternal(nameof(PersonName)) == null &&
                GetErrorInternal(nameof(Money)) == null &&
                GetErrorInternal(nameof(Note)) == null;
        }
        protected override string GetErrorInternal(string columnName)
        {
            switch (columnName)
            {
                case nameof(PersonName):
                    if (string.IsNullOrEmpty(PersonName))
                        return "Имя не должно быть пустым";
                    break;
                case nameof(Money):
                    if (Money < 0)
                        return "Значение не должно быть ниже нуля";
                    break;
            }
            return null;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Income Clone()
        {
            return new Income
            {
                Money = Money,
                IncomeDate = IncomeDate,
                Note = Note,
                PersonName = PersonName,
            };
        }
    }

    public enum ExpenseType
    {
        Salary, Tax, Foodstuff, Utilities, Private, Other,
    }

    public enum Sex : byte { Male = 0, Female = 1 }

    public enum Parents : byte { Father = 0, Mother = 1, Other = 2 }
}
