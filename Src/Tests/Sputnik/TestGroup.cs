using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

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
        private int? failed;

        public TestGroup(TestGroup root, Sputnik.TestCase testCase)
        {
            this.selected = false;
            this.root = root;
            this.TestCase = testCase;
            this.TestGroups = new TestGroup[0];
        }

        public IList<TestGroup> TestGroups { get; set; }

        public string Name { get; set; }

        public Visibility ShowChildrenMenu
        {
            get
            {
                return this.TestGroups != null && this.TestGroups.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public Visibility ShowTestCaseMenu
        {
            get
            {
                return this.TestGroups != null && this.TestGroups.Count > 0
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

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
                if (!this.failed.HasValue)
                {
                    return Status.Unknown;
                }
                else if (this.failed.Value == 0)
                {
                    return Status.Passed;
                }
                else
                {
                    return Status.Failed;
                }
            }
        }

        public int? Failed
        {
            get
            {
                return this.failed;
            }

            set
            {
                this.failed = value;
                NotifyFailedChanged();
                if (this.root != null)
                {
                    this.root.UpdateFailed();
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

        private void NotifyFailedChanged()
        {
            var e = this.PropertyChanged;
            if (e != null)
            {
                e(this, new PropertyChangedEventArgs("Failed"));
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

        private void UpdateFailed()
        {
            var newFailed = this.DetermineFailed();
            if (newFailed != this.failed)
            {
                this.failed = newFailed;
                this.NotifyFailedChanged();
                if (this.root != null)
                {
                    this.root.UpdateFailed();
                }
            }
        }

        private int? DetermineFailed()
        {
            if (this.TestGroups == null || this.TestGroups.Count == 0)
            {
                return this.failed;
            }

            var failed = this.TestGroups.Where(g => g.failed.HasValue && g.failed.Value > 0).Sum(g => g.failed.Value);

            if (failed > 0)
            {
                return failed;
            }

            if (this.TestGroups.Where(g => !g.failed.HasValue).Any())
            {
                return null;
            }

            return 0;
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
