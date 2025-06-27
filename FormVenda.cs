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
    public partial class FormVenda : Form
    {
        public static FormVenda venda;

        public FormVenda()
        {
            InitializeComponent();
            VendaAtual();
            CarregarDados();
            venda = this;
        }

        //chamando a variavel com o endereço da conexão
        private readonly string connectionString = Program.connectionString;
        public string id;
        public void CarregarDados()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT i.id, p.tipo, p.marca, p.descricao, c.cor, i.preco, i.qtd FROM Produtos p LEFT JOIN item_venda i ON p.id = i.id_produto LEFT JOIN cor_produto c ON i.id_cor = c.id AND i.id_produto = c.id_produto WHERE i.id_venda = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                    }
                }
                PrecoTotal();
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VendaAtual()
        {
            try
            {
                string finalizada = "False";
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, finalizada FROM venda";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            id = lastRow[0].ToString();
                            finalizada = lastRow[1].ToString();
                        }
                        else
                        {
                            finalizada = "True";
                        }
                    }
                    if (finalizada == "True")
                    {
                        string query2 = "USE Projeto_G4 INSERT INTO venda (finalizada) VALUES (@finalizada)";
                        using (SqlCommand cmd = new SqlCommand(query2, conn))
                        {
                            cmd.Parameters.AddWithValue("@finalizada", 0);
                            cmd.ExecuteNonQuery();
                        }
                        string query3 = "USE Projeto_G4 SELECT id FROM venda";
                        using (SqlCommand cmd = new SqlCommand(query3, conn))
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            id = lastRow[0].ToString();
                        }
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimparCampos()
        {
            txtProduto.Clear();
            txtQntd.Clear();
            txtPagamento.Text = "Dinheiro";
            txtParcela.Text = "1x";
            txtTotal.Clear();
        }

        private Boolean IdInvalido() //verifica se id é valido
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT i.id, p.tipo, p.marca, p.descricao, c.cor, p.preco, i.qtd FROM Produtos p LEFT JOIN item_venda i ON p.id = i.id_produto LEFT JOIN cor_produto c ON i.id_venda = c.id WHERE i.id = @id AND i.id_venda = @id_venda";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtProduto.Text);
                        cmd.Parameters.AddWithValue("@id_venda", id);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() != "0") //se id for valido retorna false
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

        private void NotaFiscal()
        {
            FormNotaV formNotaV = new FormNotaV();
            formNotaV.ShowDialog();
        }

        private void PrecoTotal()
        {
            try
            {
                string preco, preco2;
                float precoconvertido, precototal = 0;
                if (dataGridView1.RowCount < 1) return;
                for (int linha = 0; linha < dataGridView1.RowCount; linha++)
                {
                    preco2 = "";
                    preco = dataGridView1.Rows[linha].Cells[5].Value.ToString();
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
                    precoconvertido = float.Parse(preco2, CultureInfo.InvariantCulture.NumberFormat);
                    precototal += precoconvertido;
                }
                txtTotal.Text = "R$ " + precototal.ToString();
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ConverteParcela()
        {
            try
            {
                string parcela = "";
                foreach (char c in txtParcela.Text)
                {
                    if (char.IsDigit(c)) parcela += c;
                    else continue;
                }
                return int.Parse(parcela);
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("Nenhum produto inserido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //linhas abaixo confirmam se compra deve ser cancelada
            const string pergunta = "Tem certeza que deseja cancelar a venda?";
            var escolha = MessageBox.Show(pergunta, "Deletando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No)
            {
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 DELETE FROM item_venda WHERE id_venda = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LimparCampos(); //limpa todas txtbox
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private string CalculaPreco()
        {
            try
            {
                string id_produto, preco, preco2 = "";
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT id_produto FROM item_venda WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtProduto.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                        id_produto = lastRow[0].ToString();
                    }
                    string query2 = "USE Projeto_G4 SELECT preco FROM Produtos WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id_produto);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                        preco = lastRow[0].ToString();
                    }
                }
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
                float qtd = int.Parse(txtQntd.Text);
                float precofinal = precoflt * qtd;
                return "R$ " + precofinal.ToString();
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "R$ 1,00";
        }

        private bool Checar_Vazio()
        {
            if (txtParcela.Text == "" || txtPagamento.Text == "" || txtTotal.Text == "")
            {
                MessageBox.Show("Um ou mais campos estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }

        private bool ChecarCpf() //checa se todos os 14 caracteres foram preenchidos
        {
            if (txtCPF.Text == "   ,   ,   -") return true;
            else if (txtCPF.Text.Length < 14)
            {
                return false;
            }
            else return true;
        }

        private string Cpf()
        {
            if (txtCPF.Text == "   ,   ,   -") return "null";
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        string query = "USE Projeto_G4 SELECT id FROM clientes WHERE cpf = @cpf";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@cpf", txtCPF.Text);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                                return lastRow[0].ToString();
                            }
                            else return "erro";
                        }
                    }
                }
                catch (Exception ex) //caso der erro, mostra a mensagem de erro
                {
                    MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return "erro";
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("Nenhum produto inserido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return; //se um campo estiver vazio, não faz o insert
            //so faz o insert se cpf e id ainda nao existirem no bd e se o cpf for valido (14 caracteres)
            if (!ChecarCpf() || Cpf() == "erro")
            {
                MessageBox.Show("Cpf inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //linhas abaixo confirmam se a compra deve ser efetuada
            const string pergunta = "Finalizar venda?";
            var escolha = MessageBox.Show(pergunta, "Finalizando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No)
            {
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                string id_item, id_produto, id_cor, qtd, qtdantigo = "0";
                int qtdi = 0, id_cliente;
                bool temnoestoque = false;
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    for (int linha = 0; linha < dataGridView1.RowCount; linha++)
                    {
                        id_item = dataGridView1.Rows[linha].Cells[0].Value.ToString();
                        string queryselect = "USE Projeto_G4 SELECT id_produto, id_cor, qtd FROM item_venda WHERE id = @id AND id_venda = @id_venda";
                        using (SqlCommand cmd = new SqlCommand(queryselect, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id_item);
                            cmd.Parameters.AddWithValue("@id_venda", id);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            id_produto = lastRow[0].ToString();
                            id_cor = lastRow[1].ToString();
                            qtd = lastRow[2].ToString();
                        }
                        string queryqtd = "USE Projeto_G4 SELECT qtd FROM Estoque WHERE id_cor = @id_cor AND id_produto = @id_produto";
                        using (SqlCommand cmd = new SqlCommand(queryqtd, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_cor", id_cor);
                            cmd.Parameters.AddWithValue("@id_produto", id_produto);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                                qtdantigo = lastRow[0].ToString();
                                temnoestoque = true;
                            }
                        }
                        qtdi = int.Parse(qtd);
                        if (!int.TryParse(qtdantigo, out int qtdantigoi)) qtdantigoi = 0;
                        qtdi = qtdantigoi - qtdi;
                        if (!temnoestoque || qtdi < 0)
                        {
                            MessageBox.Show("Quantidade insuficiente no estoque", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    for (int linha2 = 0; linha2 < dataGridView1.RowCount; linha2++)
                    {
                        id_item = dataGridView1.Rows[linha2].Cells[0].Value.ToString();
                        string queryselect2 = "USE Projeto_G4 SELECT id_produto, id_cor, qtd FROM item_venda WHERE id = @id AND id_venda = @id_venda";
                        using (SqlCommand cmd = new SqlCommand(queryselect2, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id_item);
                            cmd.Parameters.AddWithValue("@id_venda", id);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                            id_produto = lastRow[0].ToString();
                            id_cor = lastRow[1].ToString();
                            qtd = lastRow[2].ToString();
                        }
                        string queryqtd2 = "USE Projeto_G4 SELECT qtd FROM Estoque WHERE id_cor = @id_cor AND id_produto = @id_produto";
                        using (SqlCommand cmd = new SqlCommand(queryqtd2, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_cor", id_cor);
                            cmd.Parameters.AddWithValue("@id_produto", id_produto);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
                                qtdantigo = lastRow[0].ToString();
                                temnoestoque = true;
                            }
                        }
                        qtdi = int.Parse(qtd);
                        if (!int.TryParse(qtdantigo, out int qtdantigoi2)) qtdantigoi2 = 0;
                        qtdi = qtdantigoi2 - qtdi;
                        string queryestoque = "USE Projeto_G4 UPDATE Estoque SET qtd = @qtd WHERE id_cor = @id_cor AND id_produto = @id_produto";
                        using (SqlCommand cmd = new SqlCommand(queryestoque, conn))
                        {
                            cmd.Parameters.AddWithValue("@qtd", qtdi);
                            cmd.Parameters.AddWithValue("@id_cor", id_cor);
                            cmd.Parameters.AddWithValue("@id_produto", id_produto);
                            cmd.ExecuteNonQuery();
                        }

                    }
                    if (Cpf() != "null")
                    {
                        id_cliente = int.Parse(Cpf());
                        string querycliente = "USE Projeto_G4 INSERT INTO venda_cliente (id_venda, id_cliente) VALUES (@id_venda, @id_cliente)";
                        using (SqlCommand cmd = new SqlCommand(querycliente, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_venda", id);
                            cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    string query = "USE Projeto_G4 INSERT INTO pag_venda (id, preco_final, pagamento, parcela) VALUES (@id, @preco_final, @pagamento, @parcela)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@preco_final", txtTotal.Text);
                        cmd.Parameters.AddWithValue("@pagamento", txtPagamento.Text);
                        cmd.Parameters.AddWithValue("@parcela", ConverteParcela());
                        cmd.ExecuteNonQuery();
                    }
                    string query2 = "USE Projeto_G4 UPDATE venda SET finalizada = @finalizada, datahora = @datahora WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        //finaliza a compra atual
                        cmd.Parameters.AddWithValue("@finalizada", 1);
                        cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    if (!Form1.form1.admin)
                    {
                        string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                        using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                            cmd.Parameters.AddWithValue("@acao", "Venda");
                            cmd.Parameters.AddWithValue("@descricao", "Venda: " + id);
                            cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                LimparCampos();
                NotaFiscal();
                VendaAtual();
                CarregarDados();
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProduto_Click(object sender, EventArgs e)
        {
            FormPesquisaV formPesquisaV = new FormPesquisaV();
            formPesquisaV.ShowDialog();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int linha = Int32.Parse(e.RowIndex.ToString()); //pega numero da linha selecionada
                if (linha >= 0) //garante q linha seleciona seja maior q zero
                {
                    //preenche as txtbox com os valores da linha selecionada no gridview
                    txtProduto.Text = dataGridView1.Rows[linha].Cells[0].Value.ToString();
                    txtQntd.Text = dataGridView1.Rows[linha].Cells[6].Value.ToString();
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtProduto.Text == "")
            {
                MessageBox.Show("Nenhum produto selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtProduto.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código do Produto deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (IdInvalido()) //precisa de um id valido no campo codigo
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            //linhas abaixo confirmam se a exclusao deve ser efetuada
            const string pergunta = "Tem certeza que deseja excluir este produto da venda?";
            var escolha = MessageBox.Show(pergunta, "Excluir...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No)
            {
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 DELETE FROM item_venda WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtProduto.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LimparCampos(); //limpa todas txtbox
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void btnAtualiza_Click(object sender, EventArgs e)
        {
            if (txtProduto.Text == "")
            {
                MessageBox.Show("Nenhum produto selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtProduto.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código do Produto deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtQntd.Text.ToString(), out int i) || i < 1) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("Quantidade inválida", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (IdInvalido()) //precisa de um id valido no campo codigo
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 UPDATE item_venda SET qtd = @qtd, preco = @preco WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@qtd", txtQntd.Text);
                        cmd.Parameters.AddWithValue("@preco", CalculaPreco());
                        cmd.Parameters.AddWithValue("@id", txtProduto.Text);
                        cmd.ExecuteNonQuery();
                    }
                    LimparCampos(); //limpa todas txtbox
                    CarregarDados(); //preenche o gridview com os dados do bd
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
