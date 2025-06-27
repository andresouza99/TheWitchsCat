using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    public partial class FormPesquisaV : Form
    {
        public FormPesquisaV()
        {
            InitializeComponent();
            CarregarDados();
        }

        //chamando a variavel com o endereço da conexão
        private readonly string connectionString = Program.connectionString;
        readonly string id = FormVenda.venda.id;

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT * FROM Produtos";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                    }
                    string query2 = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt;
                        dataGridView2.AutoResizeColumn(0);
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparCampos() //limpa todas txtbox
        {
            txtCodigo.Clear();
            txtMarca.Clear();
            txtTipo.Clear();
            txtDescricao.Clear();
            txtCor.Clear();
        }
       
        private bool Checar_Vazio() //checa se um dos campos obrigatorios esta vazio
        {
            if (txtMarca.Text == "" || txtTipo.Text == "" || txtDescricao.Text == "" || txtCor.Text == "")
            {
                MessageBox.Show("Um ou mais campos estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }
        
        private Boolean IdInvalido() //verifica se id é valido
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT * FROM Produtos WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() == "1") //se id for valido retorna false
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private int IdItem()
        {
            try
            {
                int i = 1;
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    while (i != 0) //verifica de 1 em 1 se o id esta disponivel
                    {
                        string query = "USE Projeto_G4 SELECT * FROM item_venda WHERE id = @id AND id_venda = @id_venda";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", i);
                            cmd.Parameters.AddWithValue("@id_venda", id);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0) i++; //se id esta em uso, continua o loop
                            else return i; //retorna id disponivel
                        }
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCodigo.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return;//se um campo estiver vazio, não faz o insert
            if (IdInvalido()) //precisa de um id valido para fazer o insert
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                string id_cor = "", preco = "", id2 = "", qtd = "";
                bool achouacor = false, achouproduto = false;
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE cor = @cor AND id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            id_cor = lastRow[0].ToString();
                            achouacor = true;
                        }
                    }
                    if (achouacor)
                    {
                        string query2 = "USE Projeto_G4 SELECT id, qtd FROM item_venda WHERE id_venda = @id_venda AND id_produto = @id_produto AND id_cor = @id_cor";
                        using (SqlCommand cmd = new SqlCommand(query2, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_venda", id);
                            cmd.Parameters.AddWithValue("@id_produto", txtCodigo.Text);
                            cmd.Parameters.AddWithValue("@id_cor", id_cor);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow lastRow2 = dt.Rows[dt.Rows.Count - 1];
                                id2 = lastRow2[0].ToString();
                                qtd = lastRow2[1].ToString();
                                achouproduto = true;
                            }
                        }
                        string query3 = "USE Projeto_G4 SELECT preco FROM Produtos WHERE id = @id";
                        using (SqlCommand cmd = new SqlCommand(query3, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            preco = lastRow[0].ToString();
                        }
                        if (achouproduto)
                        {
                            int qtdi = int.Parse(qtd);
                            qtdi++;
                            qtd = qtdi.ToString();
                            string preco2 = "";
                            foreach (char c in preco)
                            {
                                if (char.IsDigit(c))
                                {
                                    preco2 += c;
                                }
                                else if (c.Equals(','))
                                {
                                    preco2 += ".";
                                }
                                else continue;
                            }
                            float precoflt = float.Parse(preco2, CultureInfo.InvariantCulture.NumberFormat);
                            precoflt *= qtdi;
                            preco = "R$ " + precoflt.ToString();
                            string queryupdate = "USE Projeto_G4 UPDATE item_venda SET qtd = @qtd, preco = @preco WHERE id = @id";
                            using (SqlCommand cmd = new SqlCommand(queryupdate, conn))
                            {
                                cmd.Parameters.AddWithValue("@qtd", qtd);
                                cmd.Parameters.AddWithValue("@preco", preco);
                                cmd.Parameters.AddWithValue("@id", id2);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string query4 = "USE Projeto_G4 INSERT INTO item_venda (id, id_venda, id_produto, id_cor, qtd, preco) VALUES (@id, @id_venda, @id_produto, @id_cor, @qtd, @preco)";
                            using (SqlCommand cmd = new SqlCommand(query4, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", IdItem());
                                cmd.Parameters.AddWithValue("@id_venda", id);
                                cmd.Parameters.AddWithValue("@id_produto", txtCodigo.Text);
                                cmd.Parameters.AddWithValue("@id_cor", id_cor);
                                cmd.Parameters.AddWithValue("@qtd", 1);
                                cmd.Parameters.AddWithValue("@preco", preco);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        FormVenda.venda.CarregarDados();
                        this.Close();
                    }
                    else MessageBox.Show("Cor indisponível para esse produto", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            CarregarDados();
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
                    string query = "USE Projeto_G4 SELECT * FROM Produtos WHERE id LIKE @id AND marca LIKE @marca AND tipo LIKE @tipo AND descricao LIKE @descricao";
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
                        dataGridView1.AutoResizeColumn(0);
                    }
                    string query2 = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id_produto LIKE @id AND cor LIKE @cor";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", "%" + txtCodigo.Text + "%");
                        cmd.Parameters.AddWithValue("@cor", "%" + txtCor.Text + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt;
                        dataGridView2.AutoResizeColumn(0);
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int linha = Int32.Parse(e.RowIndex.ToString()); //pega numero da linha selecionada
                if (linha >= 0) //garante q linha seleciona seja maior q zero
                {
                    //preenche as txtbox com os valores da linha selecionada no gridview
                    txtCodigo.Text = dataGridView1.Rows[linha].Cells[0].Value.ToString();
                    txtMarca.Text = dataGridView1.Rows[linha].Cells[1].Value.ToString();
                    txtTipo.Text = dataGridView1.Rows[linha].Cells[2].Value.ToString();
                    txtDescricao.Text = dataGridView1.Rows[linha].Cells[4].Value.ToString();
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id_produto = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView2.DataSource = dt;
                            dataGridView2.AutoResizeColumn(0);
                        }
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int linha = Int32.Parse(e.RowIndex.ToString()); //pega numero da linha selecionada
                if (linha >= 0) //garante q linha seleciona seja maior q zero
                {
                    //preenche as txtbox com os valores da linha selecionada no gridview
                    txtCor.Text = dataGridView2.Rows[linha].Cells[1].Value.ToString();
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
