using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace eRotulo.Negocio
{
    class ImpressoraClass
    {
        private SerialPort porta = new SerialPort(); 

        public ImpressoraClass()
        {

            //SerialPort porta = new SerialPort();
            //port.Open();
    	    //port.Write(esc+"@hola mundo!"+ff);
    	    //port.Close();
    }

        public void portaConfiguracao(string portaCOM, int boudRate, Parity paridade, int dataBits, StopBits stopBits, Encoding codificacao)
        {
            porta.PortName = portaCOM;
            porta.BaudRate = boudRate;
            porta.Parity = paridade;
            porta.DataBits = dataBits;
            porta.StopBits = stopBits;
            porta.Encoding = codificacao; //Encoding.ASCII;  
        }

        public void portaAbrir()
        {
            porta.Open();
        }

        public void portaFechar()
        {
            porta.Close();
        }

        public void imprimir(string texto)
        {
            porta.Write(texto); 
        }

        public void enviarComando(string texto)
        {
            porta.WriteLine(texto);
        }


    }
}
