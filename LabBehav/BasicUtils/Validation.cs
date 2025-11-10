using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BasicUtils;

namespace BasicUtils
{
    public interface IRule
    {
        // Properties
        string Description { get; }
        string Name { get; }
    }



    public interface IValidatedObject
    {
        // Methods
        IEnumerable<IRule> Validate();

        // Properties
        bool IsValid { get; }
    }

    public interface IBusinessRule<T> : IRule
    {
        // Methods
        bool IsSatisfiedBy(T item);
    }

 
    public interface IBusinessRuleSet
    {
        // Methods
        IEnumerable<IRule> BrokenBy(IValidatedObject item);
        bool Contains(IRule rule);

        // Properties
        int Count { get; }
        bool IsEmpty { get; }
        IList<string> Messages { get; }
    }

    public class BusinessRule<T> : IBusinessRule<T>, IRule where T : IValidatedObject
    {
        // Fields
        private string description;
        private Predicate<T> matchPredicate;
        private string name;

        // Methods
        public BusinessRule(string name, string description, Predicate<T> matchPredicate)
        {
            this.name = name;
            this.description = description;
            this.matchPredicate = matchPredicate;
        }

        public override bool Equals(object obj)
        {
            IBusinessRule<T> rule = obj as IBusinessRule<T>;
            if (rule != null)
            {
                return this.Name.Equals(rule.Name);
            }
            return true;
        }

        public override int GetHashCode()
        {
            return (this.matchPredicate.GetHashCode() + (0x1d * this.name.GetHashCode()));
        }

        public bool IsSatisfiedBy(T item)
        {
            return this.matchPredicate(item);
        }

        // Properties
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }

    public class BusinessRuleSet<T> : IBusinessRuleSet where T : IValidatedObject
    {
        // Fields
        private IList<IBusinessRule<T>> rules;

        // Methods
        public BusinessRuleSet(params IBusinessRule<T>[] rules)
            : this(new List<IBusinessRule<T>>(rules))
        {
        }

        public BusinessRuleSet(IList<IBusinessRule<T>> rules)
        {
            this.rules = rules;
        }

        public IEnumerable<IRule> BrokenBy(IValidatedObject item)
        {
            IList<IRule> list = new List<IRule>();
            foreach (IBusinessRule<T> rule in this.rules)
            {
                if (!rule.IsSatisfiedBy((T)item))
                {
                    list.Add(rule);
                }
            }
            return list;
        }

        public bool Contains(IRule rule)
        {
            foreach (IBusinessRule<T> rule2 in this.rules)
            {
                if (rule.Name.Equals(rule2.Name))
                {
                    return true;
                }
            }
            return false;
        }

        // Properties
        public int Count
        {
            get
            {
                return this.rules.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        public IList<string> Messages
        {
            get
            {
                return new List<IBusinessRule<T>>(this.rules).ConvertAll<string>(delegate(IBusinessRule<T> rule)
                {
                    return rule.Description;
                });
            }
        }
    }


    public class ValidatedDomainObject : IValidatedObject
    {
        // Methods
        public IEnumerable<IRule> Validate()
        {
            return this.Rules.BrokenBy(this);
        }

        // Properties
        public bool IsValid
        {
            get
            {
                IEnumerable<IRule> ret = Validate();
                return Validate().IsEmpty();
            }
        }

        protected virtual IBusinessRuleSet Rules
        {
            get;set;
        }
    }
}
