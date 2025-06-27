using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    public partial class Form1 : Form
    {
        //torna publico o valor da txtbox Usuario para alterar label no menu
        public static Form1 form1;
        public TextBox usuario;
        public Boolean admin = false;
        public int user;

        public Form1()
        {
            InitializeComponent();
            form1 = this;
            usuario = txtUser;
            admin = false;
        }

        //chamando a variavel com o endereço da conexão
        private readonly string connectionString = Program.connectionString;

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            //possibilita logar como admin
            if (txtUser.Text == "admin" && txtSenha.Text == "1234")
            {   
                admin = true;
                Form2 form2 = new Form2();
                form2.Show();
                this.Hide();
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        string tipo = "";
                        string query = "USE Projeto_G4 SELECT id, tipo FROM funcionarios WHERE usuario = @usuario AND senha = @senha";
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@usuario", txtUser.Text);
                            cmd.Parameters.AddWithValue("@senha", txtSenha.Text);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                user = reader.GetInt32(0);
                                tipo = reader.GetString(1);
                            }
                            reader.Close();
                        }
                        if (tipo == "Gerente")
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Login");
                                cmd.Parameters.AddWithValue("@descricao", "Login");
                                cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                            Form2 form2 = new Form2();
                            form2.Show();
                            this.Hide();
                        }
                        else if (tipo == "Vendedor")
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Login");
                                cmd.Parameters.AddWithValue("@descricao", "Login");
                                cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                            FormVendedor formVendedor = new FormVendedor();
                            formVendedor.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou Senha Inválidos!", "Falha no Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex) //caso der erro, mostra a mensagem de erro
                {
                    MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
        }
    }
}
