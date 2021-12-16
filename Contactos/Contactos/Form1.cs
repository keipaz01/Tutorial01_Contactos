using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contactos
{
    public partial class FormAgenda : Form
    {
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=ContactosDB;User Id=sa;Password=sa;";
        int ContactosID = 0;
        public FormAgenda()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombreCompleto.Text.Trim() != "" && txtDireccion.Text.Trim() != "" && txtEmail.Text.Trim() != "")
            {
                Regex reg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = reg.Match(txtEmail.Text.Trim());
                if (match.Success) // If validation is success: if (match.Success)
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        SqlCommand sqlCmd = new SqlCommand("ContactAddOrEdit", sqlCon);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@ContactosID", ContactosID);
                        sqlCmd.Parameters.AddWithValue("@NombreCompleto", txtNombreCompleto.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Telefono", txtTel.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Pais", 1);
                        sqlCmd.Parameters.AddWithValue("@Provincia", 2);
                        sqlCmd.ExecuteNonQuery();
                        MessageBox.Show("Contacto Ingresado.");
                        Clear();
                        GridFill();

                        txtEmail.BackColor = SystemColors.Window;
                    }
                }
                else
                {
                    MessageBox.Show("Dirección de Correo Inválida.");
                    txtEmail.Focus();
                    txtEmail.BackColor = Color.MistyRose;
                }
            }
            else
                MessageBox.Show("Por favor llene campo/s obligatorio/s.");

        }

        void Clear()
        {
            txtNombreCompleto.Text = txtDireccion.Text = txtEmail.Text = txtTel.Text = cmbPais.Text = cmbProv.Text = txtBuscar.Text = "";
            ContactosID = 0;
            btnGuardar.Text = "Guardar";
            btnBorrar.Enabled = false;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Clear();
        }
        void GridFill() //Function
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                // In order to retrieve results from SQL Server DB, we have to use:
                SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewAll",sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                // We can fill the SQL result into this C# data table by calling the Fill function.
                sqlDa.Fill(dtbl); // As the parameter for this function we will pass dtbl to fill the data.
                dgvContactos.DataSource = dtbl;
            }
        }
        /*
        public class Parametro
        {
            public int ID;
            public string Nombre;
        }

        public class ClaseProvincia : Parametro
        { }

        public class ClasePais : Parametro
        { }

        public class Contacto
        {
            public int ConctactoID;
            public string NombreCompleto;
            public string Direccion;
            public string Telefono;
            public string Email;
            public ClaseProvincia Provincia;
            public ClasePais Pais;
        }
        */
        private void FormAgenda_Load(object sender, EventArgs e)
        {
            txtNombreCompleto.Select();

            /* Contacto myContacto = new Contacto();

            myContacto.ConctactoID = 1;
            myContacto.NombreCompleto = "Damian";
            myContacto.Direccion = "paysandu";
            myContacto.Telefono = "123";
            myContacto.Email = "email";
            myContacto.Pais = new ClasePais();
            myContacto.Pais.ID = 23;
            myContacto.Pais.Nombre = "Soy un pais";
            myContacto.Provincia = new ClaseProvincia();
            myContacto.Provincia.ID = 55;
            

            MessageBox.Show(myContacto.Pais.Nombre);
            */




            GridFill();
            btnBorrar.Enabled = false;
        }

        private void dgvContactos_DoubleClick(object sender, EventArgs e)
        {
            if(dgvContactos.CurrentRow.Index != -1)
            {
                txtNombreCompleto.Text = dgvContactos.CurrentRow.Cells[1].Value.ToString();
                txtDireccion.Text = dgvContactos.CurrentRow.Cells[2].Value.ToString();
                txtEmail.Text = dgvContactos.CurrentRow.Cells[3].Value.ToString();
                txtTel.Text = dgvContactos.CurrentRow.Cells[4].Value.ToString();
                cmbPais.Text = dgvContactos.CurrentRow.Cells[5].Value.ToString();
                cmbProv.Text = dgvContactos.CurrentRow.Cells[6].Value.ToString();
                ContactosID = Convert.ToInt32(dgvContactos.CurrentRow.Cells[0].Value.ToString());

                // After I doubleclick on a column, the botton "Guardar" will change to "Actualizar".
                btnGuardar.Text = "Actualizar";
                btnBorrar.Enabled = true;
            }

        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand("ContactDeleteByID", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@ContactosID", ContactosID);                
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Borrado Existosamente");
                Clear();
                GridFill();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                // In order to retrieve results from SQL Server DB, we have to use:
                SqlDataAdapter sqlDa = new SqlDataAdapter("ContactSearchByValue", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("@SearchValue", txtBuscar.Text.Trim());
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                dgvContactos.DataSource = dtbl;
            }
        }

        private void txtTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        
    }
}
