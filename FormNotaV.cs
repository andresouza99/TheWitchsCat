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
    public partial class FormNotaV : Form
    {
        public FormNotaV()
        {
            InitializeComponent();
            CarregarDados();
        }

        //chamando a variavel com o endereço da conexão
        private readonly string connectionString = Program.connectionString;
        readonly string id = FormVenda.venda.id;

        private void CarregarDados() //Método para carregar OS DADOS do Banco de dados na ListBox
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) // Cria uma conexão com o banco 
            {
                conn.Open(); // Abre a conexão com o banco
                string query = "USE Projeto_G4 SELECT datahora FROM venda WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox1.Items.Add(reader["datahora"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query2 = "USE Projeto_G4 SELECT qtd FROM item_venda WHERE id_venda = @id";
                using (SqlCommand cmd = new SqlCommand(query2, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox2.Items.Add(reader["qtd"]);
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query3 = "USE Projeto_G4 SELECT preco_final FROM pag_venda WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query3, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox3.Items.Add(reader["preco_final"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query4 = "USE Projeto_G4 SELECT p.tipo, p.marca, c.cor FROM Produtos p LEFT JOIN item_venda i ON p.id = i.id_produto LEFT JOIN cor_produto c ON i.id_cor = c.id AND p.id = c.id_produto WHERE i.id_venda = @id";
                using (SqlCommand cmd = new SqlCommand(query4, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox4.Items.Add(reader["tipo"].ToString() + "  " + reader["marca"].ToString() + "  " + reader["cor"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query5 = "USE Projeto_G4 SELECT p.preco FROM Produtos p LEFT JOIN item_venda i ON i.id_produto = p.id WHERE i.id_venda = @id";
                using (SqlCommand cmd = new SqlCommand(query5, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox5.Items.Add(reader["preco"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query6 = "USE Projeto_G4 SELECT preco FROM item_venda WHERE id_venda = @id";
                using (SqlCommand cmd = new SqlCommand(query6, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox6.Items.Add(reader["preco"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query7 = "USE Projeto_G4 SELECT id FROM venda WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query7, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox7.Items.Add("Código: " + reader["id"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query8 = "USE Projeto_G4 SELECT c.cliente FROM clientes c LEFT JOIN venda_cliente v ON c.id = v.id_cliente WHERE v.id_venda = @id";
                using (SqlCommand cmd = new SqlCommand(query8, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox8.Items.Add("Cliente: " + reader["cliente"].ToString());
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
                string query9 = "USE Projeto_G4 SELECT pagamento,parcela FROM pag_venda WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query9, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    //Criar um comando SQL para selecionar os usuários na tabela 
                    SqlDataReader reader = cmd.ExecuteReader(); // Executa o comando e retorna um leitor de dados
                    while (reader.Read()) // Percorre os resultados retornados pela consulta
                    {
                        listBox9.Items.Add("Forma de Pagamento: " + reader["pagamento"].ToString() + "  " + "Parcelamento: " + reader["parcela"].ToString() + "x");
                        // Adiciona oas informações da tabela na ListBox
                    }
                    reader.Close();
                }
            }
        }
    }
}
