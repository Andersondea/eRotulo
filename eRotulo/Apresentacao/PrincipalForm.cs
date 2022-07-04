using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eRotulo.Negocio;
using System.Diagnostics;
using System.Reflection;

namespace eRotuloWFA
{
    public partial class PrincipalForm : Form
    {
        ImpressaoClass etiqueta = new ImpressaoClass();
        DataSet UnidadesDataSet = new DataSet("unidades_item");

        string filePath = "banco.xml";

        string rodapeEtiqueta;

        public PrincipalForm()
        {
            InitializeComponent();
            abrirBanco();
            carregarUnidades();
            carregarFiltro();
        }

        private void PrincipalForm_Load(object sender, EventArgs e)
        {
            versaoLabel.Text = String.Format("Versão {0}", AssemblyVersion); 
            rodapeEtiqueta = "RODA PE DA ETRIQUETA";
        }

        private void filtrarButton_Click(object sender, EventArgs e)
        {
            construirFiltro();

        }

        private void sairButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("NÃO ESQUEÇA DE MUDAR A PORTA DA IMPRESSORA SROECT PARA COM1 !" + System.Environment.NewLine + System.Environment.NewLine + "CASO CONTRÁRIO, AS ETIQUETAS NÃO SERÃO IMPRESSAS A PARTIR DOS COLETORES.", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Application.Exit();
        }
        
        private void avancarButton_Click(object sender, EventArgs e)
        {
            avancarRotulo(1);
        }


        /* 
        ==========================
        Banco de Dados
        ==========================
        */
  
        private void abrirBanco()
        {
            UnidadesDataSet.ReadXml(filePath);
        }

        private void carregarUnidades()
        {
            construirFiltro();
        }

        private void carregarFiltro()
        {
            int i = 0;
            
            this.filtrarComboBox.Items.Clear();
            this.filtrarComboBox.Items.Add("EU QUERO TUDO!");
            for (i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.filtrarComboBox.Items.Add(this.dataGridView1.Columns[i].HeaderText);
            }
        }

        private void construirFiltro()
        {
            int i;
            DataTable unidades = UnidadesDataSet.Tables["unidade_item"];

            //DataGridViewTextBoxColumn novaColuna;


            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();




            if (filtrarComboBox.Text == "EU QUERO TUDO!" || filtrarComboBox.Text == "")
            {
                EnumerableRowCollection<DataRow> query = from unidade in unidades.AsEnumerable()
                                                         orderby unidade.Field<string>("unidade")
                                                         select unidade;
                DataView view = query.AsDataView();
                dataGridView1.DataSource = view;
                qtdeValorLabel.Text = view.Count.ToString();

            }

            else if (filtrarComboBox.Text == "id")
            {
                EnumerableRowCollection<DataRow> query = from unidade in unidades.AsEnumerable()
                                                         where unidade.Field<int>(filtrarComboBox.Text) == Convert.ToInt32(filtrarTextBox.Text)
                                                         orderby unidade.Field<string>("unidade")
                                                         select unidade;
                DataView view = query.AsDataView();
                dataGridView1.DataSource = view;
                qtdeValorLabel.Text = view.Count.ToString();


            }
            else if (filtrarComboBox.Text != "id")
            {
                EnumerableRowCollection<DataRow> query = from unidade in unidades.AsEnumerable()
                                                         where unidade.Field<string>(filtrarComboBox.Text).IndexOf(Convert.ToString(filtrarTextBox.Text), StringComparison.CurrentCultureIgnoreCase) >= 0
                                                         orderby unidade.Field<string>("unidade")
                                                         select unidade;
                DataView view = query.AsDataView();
                dataGridView1.DataSource = view;
                qtdeValorLabel.Text = view.Count.ToString();
            }

            for (i = 0; i < dataGridView1.Columns.Count; i++)
            {
                if (dataGridView1.Columns[i].Name.ToUpper() != "LACRE")
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
                else
                {
                    dataGridView1.Columns[i].Width = 200;
                }
            }

            
            configurarGrade();
        
        }

        private void configurarGrade()
        {
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 200;
            dataGridView1.Columns[2].Width = 40;
            dataGridView1.Columns[3].Width = 40;
            dataGridView1.Columns[4].Width = 40;
            dataGridView1.Columns[5].Width = 150;

            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        
        }

        private void imprimirButton_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt16(quantidadeTextBox.Text) > 20)
                {
                    if (MessageBox.Show("Você tem certeza que quer imprimir mais de 20 itens?", "Impressão", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        //sai
                        return;
                    }
                }

          
            
            if (unidadeTextBox.Text != "")
            {
                ImprimirIndividual();
            }
            else
            {
                ImprimirSelecionados(); 
            }
            

        }       

        /* 
        ==========================
        Tratamento de Rotulos
        ==========================
        */

        private void ImprimirIndividual()
        {

            string comandos;
            //ImpressaoClass etiqueta = new ImpressaoClass();

            setConfiguracao();

           
            
            //comandos = "<STX>R;<ETX>";
            comandos = "<STX><RS>" + quantidadeTextBox.Text + "<ETX>";
            comandos = comandos +  "<STX><ESC>E4<ETX>";
            comandos = comandos +  "<STX><CAN><ETX>";
            comandos = comandos +  "<STX>" + unidadeTextBox.Text + "<CR><ETX>";

            
            //se lacre nao for vazio imprimir com conteúdo, caso contrário um traço.
            //if (lacreTextBox.Text != "")
            //{

                comandos = comandos + "<STX>" + prefixoLacreTextBox.Text.Trim() + lacreTextBox.Text + "<CR><ETX>";
            //}
            //else
            //{
                comandos = comandos + "<STX>" + prefixoLacreTextBox.Text.Trim() + "______________________<CR><ETX>";
            //}


            comandos = comandos + "<STX>" + rodapeEtiqueta + "<CR><ETX>";
            comandos = comandos +  "<STX><ETB><ETX>";
            etiqueta.stringDeImpressao = comandos;

            etiqueta.imprimir();
           // etiqueta = null;

        }

        private void ImprimirSelecionados()
        {
            
            string comandos;
            string prefixoConvertido;
            string lacre;
            int qtdeLinhas;

            DataGridViewRow linha = new DataGridViewRow();
            

            DataGridViewSelectedRowCollection linhas; // = new DataGridViewSelectedRowCollection();
            linhas = dataGridView1.SelectedRows;
            qtdeLinhas = linhas.Count - 1;
          

            /****************************
             * COMANDOS DE CONFIGURAÇÃO *
             ****************************/

            setConfiguracao();
            
            for (int i = qtdeLinhas; i >= 0 ; i--)
            {
                linha = linhas[i]; //dataGridView1.Rows[i];


                    /****************************
                     * COMANDOS DE IMPRESSÃO    *
                     ****************************/
                    comandos = "<STX><RS>" + quantidadeTextBox.Text + "<ETX>";
                    comandos = comandos + "<STX><ESC>E4<ETX>";
                    comandos = comandos + "<STX><CAN><ETX>";


                    comandos = comandos + "<STX>" + linha.Cells[1].Value + "<CR><ETX>";
                    comandos = comandos + "<STX>" + linha.Cells[3].Value + "<CR><ETX>";
                    if (linha.Cells[5].Value.ToString() != "")
                    {
                        prefixoConvertido = "ML";
                        lacre = Convert.ToString(linha.Cells[5].Value).Substring(2);

                        if (Convert.ToString(linha.Cells[5].Value).Substring(0, 2) == "10")
                        {
                            prefixoConvertido = "CX";
                        }
                        lacre = prefixoConvertido + lacre;


                        comandos = comandos + "<STX>" + prefixoLacreTextBox.Text.Trim() + lacre + "<CR><ETX>";
                    }
                    else
                    {
                        comandos = comandos + "<STX>" + prefixoLacreTextBox.Text.Trim() + "______________________<CR><ETX>";
                    }

                    comandos = comandos + "<STX>" + rodapeEtiqueta + "<CR><ETX>";
                    comandos = comandos + "<STX><ETB><ETX>";
                
                    etiqueta.stringDeImpressao = comandos;
                    etiqueta.imprimir();
                
            }

            //etiqueta = null;
            linha = null;
            linhas = null;
        }
        
        //Avancar rotulo(s)
        private void avancarRotulo(int quantidade)
        {

            etiqueta.avancarRotulo(quantidade);
        
        }

        private void setConfiguracao()
        {

            string comandos;

            comandos = "<STX><ESC>C<ETX>";
            comandos = comandos + "<STX><ESC>P<ETX>";
            comandos = comandos + "<STX>E4;F4;<ETX>";

            comandos = comandos + "<STX>H0;o300,71;f3;c25;h20;w15;d0,35;<ETX>";
            comandos = comandos + "<STX>H1;o250,71;f3;c25;h20;w15;d0,35;<ETX>";

            
            //codigo de barras
            //comandos = comandos + "<STX>B2;o220,51;f3;c0,0;d0,20;i1;h100;w2;p@;<ETX>"; //c0,0;d0,20;i1;h100;p@;
            //texto codigo de barras
            comandos = comandos + "<STX>H2;o180,51;f3;c25;h20;w20;d0,30;<ETX>";

            comandos = comandos + "<STX>H3;o50,51;f3;c25;h10;w10;d0,35;<ETX>";

            comandos = comandos + "<STX>R;<ETX>";
            etiqueta.stringDeConfiguracao = comandos;
            etiqueta.configurar();

        }

        private void prefixoLacreTextBox_Leave(object sender, EventArgs e)
        {
            prefixoLacreTextBox.Text = prefixoLacreTextBox.Text.ToUpper(); 
        }

        private void sobreButton_Click(object sender, EventArgs e)
        {
              eRotulo.Apresentacao.sobreAboutBox  f = new eRotulo.Apresentacao.sobreAboutBox ();
              f.Show();
        }

        private void limparButton_Click(object sender, EventArgs e)
        {
            unidadeTextBox.Text = "";
            lacreTextBox.Text = "";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            UnidadesDataSet.WriteXml(filePath);

        }

        private void PodeEditarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToDeleteRows = PodeEditarCheckBox.Checked;
            dataGridView1.AllowUserToAddRows = PodeEditarCheckBox.Checked;
            dataGridView1.ReadOnly = !PodeEditarCheckBox.Checked;
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string celula = dataGridView1.SelectedCells[1].Value.ToString();
            DialogResult resposta = MessageBox.Show(celula + Environment.NewLine + 
                                                             Environment.NewLine +  
                                                             "Excluir esse registro?", "EXCLUINDO...", MessageBoxButtons.YesNo,MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (resposta == DialogResult.No)
               e.Cancel = true;
            

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            UnidadesDataSet.WriteXml(filePath);
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            UnidadesDataSet.WriteXml(filePath);
        }
       
        
        
    }
}
