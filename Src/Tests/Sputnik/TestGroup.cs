using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IronJS.Tests.Sputnik
{
    public enum Status
    {
        Unknown = 0,
        Passed,
        Failed,
    }

    public class TestGroup : INotifyPropertyChanged
    {
        private TestGroup root;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool? selected;
        private Status status;

        public TestGroup(TestGroup root, Sputnik.TestCase testCase)
        {
            this.selected = false;
            this.root = root;
            this.TestCase = testCase;
            this.TestGroups = new TestGroup[0];
        }

        public IList<TestGroup> TestGroups { get; set; }

        public string Name { get; set; }

        public bool? Selected
        {
            get
            {
                return this.selected;
            }

            set
            {
                this.SetSelected(value);
                if (this.root != null)
                {
                    this.root.UpdateSelected();
                }
            }
        }

        public Status Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                NotifyStatusChanged();
                if (this.root != null)
                {
                    this.root.UpdateStatus();
                }
            }
        }

        public TestCase TestCase { get; set; }

        private void NotifySelectedChanged()
        {
            var e = this.PropertyChanged;
            if (e != null)
            {
                e(this, new PropertyChangedEventArgs("Selected"));
            }
        }

        private void NotifyStatusChanged()
        {
            var e = this.PropertyChanged;
            if (e != null)
            {
                e(this, new PropertyChangedEventArgs("Status"));
            }
        }

        private void UpdateSelected()
        {
            var newSelected = this.DetermineSelection();
            if (newSelected != this.selected)
            {
                this.selected = newSelected;
                this.NotifySelectedChanged();
                if (this.root != null)
                {
                    this.root.UpdateSelected();
                }
            }
        }

        private bool? DetermineSelection()
        {
            if (this.TestGroups == null || this.TestGroups.Count == 0)
            {
                return this.selected;
            }

            if (this.TestGroups.Where(g => g.Selected == null).Any())
            {
                return null;
            }

            var yes = this.TestGroups.Where(g => g.Selected == true).Any();

            if (!yes)
            {
                return false;
            }

            var no = this.TestGroups.Where(g => g.Selected == false).Any();

            if (no)
            {
                return null;
            }

            return true;
        }

        private void UpdateStatus()
        {
            var newStatus = this.DetermineStatus();
            if (newStatus != this.status)
            {
                this.status = newStatus;
                this.NotifyStatusChanged();
                if (this.root != null)
                {
                    this.root.UpdateStatus();
                }
            }
        }

        private Status DetermineStatus()
        {
            if (this.TestGroups == null || this.TestGroups.Count == 0)
            {
                return this.status;
            }

            if (this.TestGroups.All(g => g.Status == Sputnik.Status.Passed))
            {
                return Sputnik.Status.Passed;
            }

            if (this.TestGroups.Where(g => g.Status == Sputnik.Status.Failed).Any())
            {
                return Sputnik.Status.Failed;
            }

            return Sputnik.Status.Unknown;
        }

        private void SetSelected(bool? value)
        {
            this.selected = value.Value;

            if (this.TestGroups != null)
            {
                foreach (var testGroup in this.TestGroups)
                {
                    testGroup.SetSelected(value);
                }
            }

            NotifySelectedChanged();
        }
    }
}
