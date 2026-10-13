using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ATKInventory
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = @"Server=localhost\SQLEXPRESS;Database=ATKInventoryDB;Integrated Security=True;TrustServerCertificate=True;";
        private string selectedId = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                this.itemsTableAdapter.Fill(this.aTKInventoryDBDataSet.Items);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.itemsTableAdapter.Fill(this.aTKInventoryDBDataSet.Items);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtItemcode.Text))
            {
                MessageBox.Show("ItemCode tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtItemname.Text))
            {
                MessageBox.Show("ItemName tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Category tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Price tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Stock tidak boleh kosong.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO dbo.items (ItemCode, ItemName, Category, Price, Stock) VALUES (@itemcode, @itemname, @category, @price, @stock)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@itemcode", txtItemcode.Text.Trim());
                        command.Parameters.AddWithValue("@itemname", txtItemname.Text.Trim());
                        command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                        command.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                        command.Parameters.AddWithValue("@stock", int.Parse(txtStock.Text));

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data pasien berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId))
            {
                MessageBox.Show("Silakan klik/pilih salah satu data pasien dari tabel terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "update dbo.Items set ItemCode = @itemcode, ItemName = @itemname, Category = @category, Price = @price, Stock = @stock where id = @id";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@itemcode", txtItemcode.Text.Trim());
                        command.Parameters.AddWithValue("@itemname", txtItemname.Text.Trim());
                        command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
                        command.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                        command.Parameters.AddWithValue("@stock", int.Parse(txtStock.Text));
                        command.Parameters.AddWithValue("@id", Convert.ToInt32(selectedId));
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data pasien dengan ID " + selectedId + " berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memperbarui: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId))
            {
                MessageBox.Show("Silakan klik/pilih salah satu data pasien dari tabel yang akan dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Apakah Anda yakin ingin menghapus data pasien dengan ID " + selectedId + "?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM dbo.Items WHERE id = @id";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", Convert.ToInt32(selectedId));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Data pasien berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menghapus: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedId = "";
            txtItemcode.Clear();
            txtItemname.Clear();
            txtCategory.Clear();
            txtPrice.Clear();
            txtStock.Clear();
        }


        private void dgvATK_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvATK.Rows.Count)
            {
                DataGridViewRow row = dgvATK.Rows[e.RowIndex];

                if (row.Cells[0].Value == null || string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
                {
                    return;
                }

                selectedId = row.Cells[0].Value.ToString();
                txtItemcode.Text = row.Cells[1].Value?.ToString() ?? "";
                txtItemname.Text = row.Cells[2].Value?.ToString() ?? "";
                txtCategory.Text = row.Cells[3].Value?.ToString() ?? "";
                txtPrice.Text = row.Cells[4].Value?.ToString() ?? "";
                txtStock.Text = row.Cells[5].Value?.ToString() ?? "";
            }
        }
    }
}


