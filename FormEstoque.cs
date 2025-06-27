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
    public partial class FormEstoque : Form
    {

        public FormEstoque()
        {
            InitializeComponent();
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        //chamando a variavel com o endereço da conexão com o bd
        private static string connectionString = Program.connectionString;

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT p.id, p.tipo, p.marca, p.preco, p.descricao, c.cor, e.qtd FROM Produtos p LEFT JOIN cor_produto c ON p.id = c.id_produto LEFT JOIN Estoque e ON p.id = e.id_produto AND c.id = e.id_cor";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumn(0);
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCodigo.Text.ToString(), out _) && txtCodigo.Text != "") //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 USE Projeto_G4 SELECT p.id, p.tipo, p.marca, p.preco, p.descricao, c.cor, e.qtd FROM Produtos p LEFT JOIN cor_produto c ON p.id = c.id_produto LEFT JOIN Estoque e ON p.id = e.id_produto AND c.id = e.id_cor WHERE p.id LIKE @id AND marca LIKE @marca AND tipo LIKE @tipo AND descricao LIKE @descricao";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", "%" + txtCodigo.Text + "%");
                        cmd.Parameters.AddWithValue("@marca", "%" + txtMarca.Text + "%");
                        cmd.Parameters.AddWithValue("@tipo", "%" + txtTipo.Text + "%");
                        cmd.Parameters.AddWithValue("@descricao", "%" + txtDescricao.Text + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos(); //limpa todas txtbox
            CarregarDados(); //preenche o gridview com os dados do bd
        }
        private void LimparCampos()
        {
            txtCodigo.Clear();
            txtMarca.Clear();
            txtTipo.Clear();
            txtDescricao.Clear();
        }
    }
}
