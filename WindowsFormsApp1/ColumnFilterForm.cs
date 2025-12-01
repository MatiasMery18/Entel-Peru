using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ColumnFilterForm : Form
    {
        readonly DataGridView grid;
        readonly DataTable data;

        CheckedListBox clbCols;
        TextBox txtColSearch;
        Button btnSelAll;
        Button btnDeselAll;
        Button btnInvert;
        ComboBox cmbCol;
        TextBox txtFilter;
        Button btnAdd;
        ListBox lstFilters;
        Button btnRemove;
        Button btnApply;
        Button btnCancel;

        Dictionary<string, string> filters = new Dictionary<string, string>();
        Dictionary<string, bool> visByHeader = new Dictionary<string, bool>();
        bool updatingList;

        public ColumnFilterForm(DataGridView grid, DataTable data)
        {
            this.grid = grid;
            this.data = data;
            Text = "Filtro de columnas";
            StartPosition = FormStartPosition.CenterParent;
            Width = 600;
            Height = 500;
            InitializeUi();
            LoadInitial();
        }

        void InitializeUi()
        {
            clbCols = new CheckedListBox { Left = 10, Top = 40, Width = 260, Height = 370 };
            txtColSearch = new TextBox { Left = 10, Top = 10, Width = 260 };
            btnSelAll = new Button { Left = 10, Top = 420, Width = 80, Text = "Todo" };
            btnDeselAll = new Button { Left = 100, Top = 420, Width = 110, Text = "Nada" };
            btnInvert = new Button { Left = 220, Top = 420, Width = 50, Text = "Inv" };
            cmbCol = new ComboBox { Left = 290, Top = 10, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            txtFilter = new TextBox { Left = 290, Top = 40, Width = 280 };
            btnAdd = new Button { Left = 290, Top = 70, Width = 120, Text = "Agregar filtro" };
            lstFilters = new ListBox { Left = 290, Top = 110, Width = 280, Height = 270 };
            btnRemove = new Button { Left = 450, Top = 70, Width = 120, Text = "Quitar filtro" };
            btnApply = new Button { Left = 290, Top = 400, Width = 120, Text = "Aplicar" };
            btnCancel = new Button { Left = 430, Top = 400, Width = 120, Text = "Cancelar" };

            btnAdd.Click += (s, e) => AddOrUpdateFilter();
            btnRemove.Click += (s, e) => RemoveSelectedFilter();
            btnApply.Click += (s, e) => { ApplyChanges(); DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            txtColSearch.TextChanged += (s, e) => RefreshColumnList();
            clbCols.ItemCheck += (s, e) =>
            {
                if (updatingList) return;
                var header = clbCols.Items[e.Index].ToString();
                visByHeader[header] = e.NewValue == CheckState.Checked;
            };
            btnSelAll.Click += (s, e) => { foreach (var k in visByHeader.Keys.ToList()) visByHeader[k] = true; RefreshColumnList(); };
            btnDeselAll.Click += (s, e) => { foreach (var k in visByHeader.Keys.ToList()) visByHeader[k] = false; RefreshColumnList(); };
            btnInvert.Click += (s, e) => { foreach (var k in visByHeader.Keys.ToList()) visByHeader[k] = !visByHeader[k]; RefreshColumnList(); };

            Controls.Add(clbCols);
            Controls.Add(txtColSearch);
            Controls.Add(btnSelAll);
            Controls.Add(btnDeselAll);
            Controls.Add(btnInvert);
            Controls.Add(cmbCol);
            Controls.Add(txtFilter);
            Controls.Add(btnAdd);
            Controls.Add(lstFilters);
            Controls.Add(btnRemove);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
        }

        void LoadInitial()
        {
            clbCols.Items.Clear();
            cmbCol.Items.Clear();
            visByHeader.Clear();
            foreach (DataGridViewColumn c in grid.Columns)
            {
                visByHeader[c.HeaderText] = c.Visible;
                clbCols.Items.Add(c.HeaderText, c.Visible);
                cmbCol.Items.Add(c.HeaderText);
            }
            if (cmbCol.Items.Count > 0) cmbCol.SelectedIndex = 0;
        }

        void RefreshColumnList()
        {
            var q = (txtColSearch.Text ?? string.Empty).Trim().ToLowerInvariant();
            updatingList = true;
            clbCols.BeginUpdate();
            clbCols.Items.Clear();
            foreach (var kv in visByHeader.ToList())
            {
                if (string.IsNullOrEmpty(q) || kv.Key.ToLowerInvariant().Contains(q))
                {
                    clbCols.Items.Add(kv.Key, kv.Value);
                }
            }
            clbCols.EndUpdate();
            updatingList = false;
        }

        void AddOrUpdateFilter()
        {
            var col = cmbCol.SelectedItem as string;
            if (string.IsNullOrEmpty(col)) return;
            var val = txtFilter.Text?.Trim() ?? string.Empty;
            if (filters.ContainsKey(col)) filters[col] = val; else filters.Add(col, val);
            RefreshFilterList();
        }

        void RemoveSelectedFilter()
        {
            if (lstFilters.SelectedItem == null) return;
            var line = lstFilters.SelectedItem.ToString();
            var idx = line.IndexOf(": ");
            if (idx > 0)
            {
                var col = line.Substring(0, idx);
                if (filters.ContainsKey(col)) filters.Remove(col);
                RefreshFilterList();
            }
        }

        void RefreshFilterList()
        {
            lstFilters.Items.Clear();
            foreach (var kv in filters)
            {
                lstFilters.Items.Add(kv.Key + ": " + kv.Value);
            }
        }

        static string EscapeLike(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("'", "''");
        }

        void ApplyChanges()
        {
            foreach (DataGridViewColumn c in grid.Columns)
            {
                if (visByHeader.TryGetValue(c.HeaderText, out var vis)) c.Visible = vis;
            }

            var parts = new List<string>();
            foreach (var kv in filters.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                var col = kv.Key;
                var val = EscapeLike(kv.Value);
                parts.Add($"CONVERT([{col}], 'System.String') LIKE '%{val}%' ");
            }
            var expr = string.Join(" AND ", parts);
            var dv = new DataView(data);
            dv.RowFilter = expr;
            grid.DataSource = dv;
        }
    }
}
