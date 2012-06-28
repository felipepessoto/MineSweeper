using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Fujiy.CampoMinado.Core
{
    public class ServidorJogo
    {
        private const int DefaultPort = 2000;
        private readonly TipoLocal[,] mapa;
        private int jogadorAtual;
        public bool Desconectado { get; set; }

        private TcpListener listener;

        private readonly Jogador[] jogadores;

        public ServidorJogo()
        {
            mapa = new TipoLocal[16, 16];

            jogadorAtual = 0;

            jogadores = new Jogador[2];

            Iniciar();
        }

        public async Task Iniciar()
        {
            listener = new TcpListener(IPAddress.Any, DefaultPort);
            listener.Start();

            SortearMapa();

            Socket jog1Socket = await listener.AcceptSocketAsync();

            jogadores[0] = new Jogador(jog1Socket, this, 0);

            Socket jog2Socket;

            try
            {
                jog2Socket = await listener.AcceptSocketAsync();
            }
            catch
            {
                jogadores[0].DesconectarJogador();
                return;
            }

            jogadores[1] = new Jogador(jog2Socket, this, 1);

            await jogadores[0].Executar();
            await jogadores[0].Ping();

            await jogadores[1].Executar();
            await jogadores[1].Ping();
            
            listener.Stop();
        }

        public void SortearMapa()
        {
            //Zera o mapa
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    mapa[x, y] = TipoLocal.Fechado;
                }
            }

            //Coloca as bombas
            Random sorteio = new Random();
            for (int x = 0; x < 52; x++)
            {
                int localsorteadoX;
                int localsorteadoY;
                do
                {
                    localsorteadoX = sorteio.Next(0, 15);
                    localsorteadoY = sorteio.Next(0, 15);
                } while (mapa[localsorteadoX, localsorteadoY] == TipoLocal.Bomba);
                mapa[localsorteadoX, localsorteadoY] = TipoLocal.Bomba;
            }


        }

        public bool EstaFechado(int localX, int localY)
        {
            TipoLocal local = mapa[localX, localY];
            return local != TipoLocal.Aberto && local != TipoLocal.BombaAberta;
        }

        public void AbrirEmVolta(int localX, int localY)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (localX + x >= 0 && localX + x <= 15 && localY + y >= 0 && localY + y <= 15)
                    {
                        if (mapa[localX + x, localY + y] == TipoLocal.Fechado)
                        {
                            //Abre o Local
                            mapa[localX + x, localY + y] = TipoLocal.Aberto;

                            //Calcula o numero do quadrado
                            int nQuadrado = CalcularNumero(localX + x, localY + y);

                            if (nQuadrado == 0)
                                AbrirEmVolta(localX + x, localY + y);

                            //Envia a mensagem para o jogador atual
                            jogadores[jogadorAtual].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                            //E depois para o rival
                            jogadores[(jogadorAtual + 1) % 2].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                        }
                    }
                }
            }
        }

        public int CalcularNumero(int localX, int localY)
        {
            //Caso seja bomba, retorna -1
            if (mapa[localX, localY] == TipoLocal.Bomba)
                return 9;

            int total = 0;

            for(int x = -1; x<2;x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (localX + x >= 0 && localX + x <= 15 && localY + y >= 0 && localY + y <= 15)
                    {
                        if (mapa[localX + x, localY + y] == TipoLocal.Bomba || mapa[localX + x, localY + y] == TipoLocal.BombaAberta)
                            total++;
                    }
                }
            }
            return total;

        }

        public bool JogadaValida(int localX, int localY, int jogador)
        {
            lock (this)
            {
                while (jogador != jogadorAtual)
                    Monitor.Wait(this);

                if (EstaFechado(localX, localY))
                {
                    //Calcula o numero do quadrado
                    int nQuadrado = CalcularNumero(localX, localY);

                    if (nQuadrado == 0)
                    {
                        jogadores[jogadorAtual].EnviarMsg((int)MensagemParaCliente.ComecaAbrirArea);
                        jogadores[(jogadorAtual + 1) % 2].EnviarMsg((int)MensagemParaCliente.ComecaAbrirArea);
                        AbrirEmVolta(localX, localY);
                        jogadores[jogadorAtual].EnviarMsg((int)MensagemParaCliente.FimAbrirArea);
                        jogadores[(jogadorAtual + 1) % 2].EnviarMsg((int)MensagemParaCliente.FimAbrirArea);
                    }

                    //Envia a mensagem para o jogador atual
                    jogadores[jogadorAtual].AbrirLocalMapa(localX, localY, nQuadrado);

                    jogadores[(jogadorAtual + 1)%2].AbrirLocalMapa(localX, localY, CalcularNumero(localX, localY) == 9 ? 10 : nQuadrado);

                    //Se acertou a bomba, adiciona o ponto
                    if (mapa[localX, localY] == TipoLocal.Bomba)
                    {
                        //Atualiza o mapa
                        mapa[localX, localY] = TipoLocal.BombaAberta;

                        //Atualiza jogador atual
                        jogadores[jogadorAtual].AdicionarPonto(jogadorAtual);
                        //Atualiza rival
                        jogadores[(jogadorAtual + 1) % 2].AdicionarPonto(jogadorAtual);
                    }
                    //Caso nao tenha acertado a bomba, passa a vez
                    else
                    {
                        //Atualiza o mapa
                        mapa[localX, localY] = TipoLocal.Aberto;

                        //Passa a vez
                        jogadorAtual = (jogadorAtual + 1) % 2;
                        jogadores[jogadorAtual].EnviarMsg((int)MensagemParaCliente.Vez);
                        jogadores[jogadorAtual].EnviarMsg(jogadorAtual);
                        jogadores[(jogadorAtual + 1) % 2].EnviarMsg((int)MensagemParaCliente.Vez);
                        jogadores[(jogadorAtual + 1) % 2].EnviarMsg(jogadorAtual);

                        //Acorda o processo do rival
                        Monitor.Pulse(this);
                    }

                    return true;
                }
                return false;
            }
        }
        
        public void Desconectar()
        {
            Desconectado = true;
            listener.Stop();
        }

        public void AmigoSaiu(int meuNumero)
        {
            jogadores[(meuNumero + 1) % 2].EnviarMsg((int)MensagemParaCliente.Amigosaiu);
        }

        public void Venceu(int numJogador)
        {
            jogadores[numJogador].EnviarMsg((int)MensagemParaCliente.Venceu);
            jogadores[(numJogador + 1) % 2].EnviarMsg((int)MensagemParaCliente.Perdeu);

            Desconectar();
        }

        public string EnderecoRemotoP0
        {
            get
            {
                if (jogadores[0] != null)
                    return jogadores[0].MeuIp;
                return "Não Conectado";
            }
        }

        public string EnderecoRemotoP1
        {
            get
            {
                if (jogadores[1] != null)
                    return jogadores[1].MeuIp;
                return "Não Conectado";
            }
        }
	
    }
}
