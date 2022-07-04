using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace eRotulo.Negocio
{
    class ImpressaoClass
    {
        ImpressoraClass imp = new ImpressoraClass();

        private string m_stringDeConfiguracao;
        private string m_stringDeImpressao;

        #region Construtores

        public ImpressaoClass()
        {

            imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            imp.portaAbrir();

        }

        public ImpressaoClass(string stringConfiguracao)
        {
            imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            imp.portaAbrir();
            m_stringDeConfiguracao = stringConfiguracao;
        }


        public ImpressaoClass(string stringConfiguracao, string stringDeImpressao)
        {
            imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            imp.portaAbrir();
            m_stringDeConfiguracao = stringConfiguracao;
            m_stringDeImpressao = stringDeImpressao;
        }

        #endregion

        #region Propriedades

        public string stringDeImpressao
        {
            get
            {
                return m_stringDeImpressao;
            }
            set
            {
                m_stringDeImpressao = value;
            }
        }

        public string stringDeConfiguracao
        {

            get
            {
                return m_stringDeConfiguracao;
            }
            set
            {
                m_stringDeConfiguracao = value;
            }
        }

        #endregion

        #region Metodos


        public void imprimir()
        {


            //imp.portaAbrir(); 

            //imp.imprimirLinha(m_stringDeConfiguracao);

            imp.enviarComando(m_stringDeImpressao);

            //imp.portaFechar();

        }

        public void imprimir(string stringComandosImpressao)
        {

            //imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            //imp.portaAbrir();

            //imp.imprimirLinha(m_stringDeConfiguracao);
            imp.enviarComando(stringDeImpressao);

        }

        public void configurar()
        {

            //ImpressoraClass imp = new ImpressoraClass();

            //imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            //imp.portaAbrir();

            imp.enviarComando(m_stringDeConfiguracao);


        }

        public void configurar(string stringComandosConfiguracao)
        {

            //ImpressoraClass imp = new ImpressoraClass();

            //imp.portaConfiguracao("COM1", 9600, Parity.None, 8, StopBits.One, Encoding.ASCII);
            //imp.portaAbrir();

            imp.enviarComando(stringComandosConfiguracao);


        }

        public void avancarRotulo(int quantidade)
        {
            string comandos;
            //ImpressaoClass etiqueta = new ImpressaoClass();

            comandos = "<STX><ESC>C<ETX>";
            comandos = comandos + "<STX><ESC>P<ETX>";
            comandos = comandos + "<STX>E4;F4;<ETX>";
            comandos = comandos + "<STX>H0;o200,51;f3;c25;h30;w20;d0,26;<ETX>";
            comandos = comandos + "<STX>H1;o180,51;f3;c25;h20;w20;d0,30;<ETX>";
            comandos = comandos + "<STX>H2;o80,51;f3;c25;h15;w15;d0,35;<ETX>";
            // this.stringDeConfiguracao = comandos;
            configurar(comandos);

            for (int i = 0; i <= quantidade; ++i)
            {
                comandos = "<STX>R;<ETX>";
                comandos = comandos + "<STX><RS>1<ETX>";
                comandos = comandos + "<STX><ESC>E4<ETX>";
                comandos = comandos + "<STX><CAN><ETX>";
                comandos = comandos + "<STX><ETB><ETX>";
                //this.stringDeImpressao = comandos;
                imprimir(comandos);
            }

        }
    }
        #endregion

}
