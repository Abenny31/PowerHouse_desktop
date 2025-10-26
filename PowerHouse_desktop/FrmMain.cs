using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PowerHouse_desktop
{
    public partial class FrmMain : Form
    {
        private readonly DataContext _dataContext;
        private int _lastKnownMaxId;
        private int _lastKnownUnreadCount;
        private bool _initialLoadCompleted;
        private bool _ignoreCellEvents;

        public FrmMain()
        {
            InitializeComponent();
            _dataContext = new DataContext();
            grdFormData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdFormData.MultiSelect = false;
            grdFormData.AutoGenerateColumns = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
                notifyIcon.Icon = SystemIcons.Information;
            }

            notifyIcon.Visible = true;
            refreshTimer.Start();
            RefreshData();
            _initialLoadCompleted = true;
        }

        private void RefreshData(bool triggeredByTimer = false)
        {
            List<FormData> forms;
            try
            {
                forms = _dataContext.formDataDbSet
                                    .AsNoTracking()
                                    .OrderByDescending(b => b.Id)
                                    .ToList();
            }
            catch (Exception ex)
            {
                refreshTimer.Stop();
                MessageBox.Show($"Failed to load data from the database.\n{ex.Message}", "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ignoreCellEvents = true;
            try
            {
                grdFormData.DataSource = forms;

                SetGridColumns();
                HighlightUnreadRows();
            }
            finally
            {
                _ignoreCellEvents = false;
            }

            var unreadCount = forms.Count(f => !f.IsRead);
            var maxId = forms.Count > 0 ? forms.Max(f => f.Id) : 0;

            UpdateUnreadIndicator(unreadCount);

            var hasNewUnread = unreadCount > _lastKnownUnreadCount;
            var hasNewEntries = maxId > _lastKnownMaxId;

            if (_initialLoadCompleted && triggeredByTimer && unreadCount > 0 && (hasNewUnread || hasNewEntries))
            {
                ShowNotification(unreadCount);
            }

            _lastKnownUnreadCount = unreadCount;
            _lastKnownMaxId = maxId;
        }

        private void SetGridColumns()
        {
            if (grdFormData.Columns.Contains("Id"))
            {
                grdFormData.Columns["Id"].Width = 50;
                grdFormData.Columns["Id"].ReadOnly = true;
            }

            if (grdFormData.Columns.Contains("Name"))
            {
                grdFormData.Columns["Name"].Width = 200;
                grdFormData.Columns["Name"].ReadOnly = true;
            }

            if (grdFormData.Columns.Contains("Email"))
            {
                grdFormData.Columns["Email"].Width = 220;
                grdFormData.Columns["Email"].ReadOnly = true;
            }

            if (grdFormData.Columns.Contains("Message"))
            {
                grdFormData.Columns["Message"].Width = 400;
                grdFormData.Columns["Message"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                grdFormData.Columns["Message"].ReadOnly = true;
            }

            if (grdFormData.Columns.Contains("TimeStamp"))
            {
                grdFormData.Columns["TimeStamp"].Width = 170;
                grdFormData.Columns["TimeStamp"].DefaultCellStyle.Format = "g";
                grdFormData.Columns["TimeStamp"].ReadOnly = true;
            }

            if (grdFormData.Columns.Contains("IsRead"))
            {
                grdFormData.Columns["IsRead"].HeaderText = "Read";
                grdFormData.Columns["IsRead"].Width = 80;
                grdFormData.Columns["IsRead"].ReadOnly = false;
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshData(true);
        }

        private void btnMarkAsRead_Click(object sender, EventArgs e)
        {
            if (grdFormData.CurrentRow?.DataBoundItem is not FormData selectedRow)
            {
                MessageBox.Show("Select a row to mark it as read.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryMarkRowAsRead(selectedRow, true))
            {
                RefreshData();
                return;
            }

            RefreshData();
        }

        private void HighlightUnreadRows()
        {
            foreach (DataGridViewRow row in grdFormData.Rows)
            {
                if (row.DataBoundItem is FormData formData && !formData.IsRead)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                    row.DefaultCellStyle.SelectionBackColor = Color.Crimson;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
                    row.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
        }

        private void UpdateUnreadIndicator(int unreadCount)
        {
            if (unreadCount <= 0)
            {
                lblUnreadIndicator.ForeColor = Color.ForestGreen;
                lblUnreadIndicator.Text = "Unread: 0";
                btnReload.BackColor = Color.ForestGreen;
                btnReload.ForeColor = Color.White;
                this.Text = "Powerhouse";
                notifyIcon.Text = "Powerhouse";
            }
            else
            {
                lblUnreadIndicator.ForeColor = Color.Red;
                lblUnreadIndicator.Text = $"Unread: {unreadCount}";
                btnReload.BackColor = Color.Red;
                btnReload.ForeColor = Color.White;
                this.Text = $"Powerhouse ({unreadCount} unread)";
                notifyIcon.Text = $"Powerhouse ({unreadCount} unread)";
            }
        }

        private void ShowNotification(int unreadCount)
        {
            var message = unreadCount == 1
                ? "You have 1 unread submission."
                : $"You have {unreadCount} unread submissions.";

            notifyIcon.BalloonTipTitle = "New Powerhouse submission";
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(5000);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            refreshTimer.Stop();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            _dataContext.Dispose();
        }

        private bool TryMarkRowAsRead(FormData selectedRow, bool showFeedbackOnFailure)
        {
            if (selectedRow == null)
            {
                if (showFeedbackOnFailure)
                {
                    MessageBox.Show("Select a row to mark it as read.", "No row selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }

            if (selectedRow.IsRead)
            {
                if (showFeedbackOnFailure)
                {
                    MessageBox.Show("The selected row is already marked as read.", "Already read", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }

            var entity = _dataContext.formDataDbSet.FirstOrDefault(f => f.Id == selectedRow.Id);
            if (entity == null)
            {
                if (showFeedbackOnFailure)
                {
                    MessageBox.Show("Unable to locate the selected record in the database.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }

            entity.IsRead = true;

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                if (showFeedbackOnFailure)
                {
                    MessageBox.Show($"Failed to mark the row as read: {ex.Message}", "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            return true;
        }

        private void grdFormData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (!string.Equals(grdFormData.Columns[e.ColumnIndex].Name, "IsRead", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            grdFormData.Rows[e.RowIndex].Selected = true;
        }

        private void grdFormData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_ignoreCellEvents)
            {
                return;
            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (!string.Equals(grdFormData.Columns[e.ColumnIndex].Name, "IsRead", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (grdFormData.Rows[e.RowIndex].DataBoundItem is not FormData selectedRow)
            {
                RefreshData();
                return;
            }

            var cellValue = grdFormData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            var isChecked = cellValue is bool b && b;

            if (!isChecked)
            {
                RefreshData();
                return;
            }

            if (!TryMarkRowAsRead(selectedRow, false))
            {
                RefreshData();
                return;
            }

            RefreshData();
        }

        private void grdFormData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grdFormData.IsCurrentCellDirty)
            {
                grdFormData.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
