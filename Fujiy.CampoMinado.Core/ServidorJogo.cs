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

            Task<Socket> jog1Socket = listener.AcceptSocketAsync();
            Task<Socket> jog2Socket = listener.AcceptSocketAsync();

            jogadores[0] = new Jogador(await jog1Socket, this, 0);
            jogadores[1] = new Jogador(await jog2Socket, this, 1);

            var taskJogador1 = jogadores[0].Executar();
            //jogadores[0].Ping();

            var taskJogador2 = jogadores[1].Executar();
            //jogadores[1].Ping();

            await Task.WhenAll(taskJogador1, taskJogador2);

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

        public async Task AbrirEmVolta(int localX, int localY)
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
                                await AbrirEmVolta(localX + x, localY + y);

                            //Envia a mensagem para o jogador atual
                            await jogadores[jogadorAtual].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                            //E depois para o rival
                            await jogadores[(jogadorAtual + 1) % 2].AbrirLocalMapa(localX + x, localY + y, nQuadrado);
                        }
                    }
                }
            }
        }

        public int CalcularNumero(int localX, int localY)
        {
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

        public async Task JogadaValida(int localX, int localY, int jogador)
        {
            if (jogador != jogadorAtual)
            {
                return;
            }

            if (EstaFechado(localX, localY))
            {
                //Calcula o numero do quadrado
                int nQuadrado = CalcularNumero(localX, localY);

                if (nQuadrado == 0)
                {
                    await jogadores[jogadorAtual].EnviarMsg(MensagemParaCliente.ComecaAbrirArea);
                    await jogadores[(jogadorAtual + 1) % 2].EnviarMsg(MensagemParaCliente.ComecaAbrirArea);
                    await AbrirEmVolta(localX, localY);
                    await jogadores[jogadorAtual].EnviarMsg(MensagemParaCliente.FimAbrirArea);
                    await jogadores[(jogadorAtual + 1) % 2].EnviarMsg(MensagemParaCliente.FimAbrirArea);
                }

                //Envia a mensagem para o jogador atual
                await jogadores[jogadorAtual].AbrirLocalMapa(localX, localY, nQuadrado);

                await jogadores[(jogadorAtual + 1) % 2].AbrirLocalMapa(localX, localY, nQuadrado == 9 ? 10 : nQuadrado);

                //Se acertou a bomba, adiciona o ponto
                if (mapa[localX, localY] == TipoLocal.Bomba)
                {
                    //Atualiza o mapa
                    mapa[localX, localY] = TipoLocal.BombaAberta;

                    //Atualiza jogador atual
                    await jogadores[jogadorAtual].AdicionarPonto(jogadorAtual);
                    //Atualiza rival
                    await jogadores[(jogadorAtual + 1)%2].AdicionarPonto(jogadorAtual);
                }
                    //Caso nao tenha acertado a bomba, passa a vez
                else
                {
                    //Atualiza o mapa
                    mapa[localX, localY] = TipoLocal.Aberto;

                    //Passa a vez
                    jogadorAtual = (jogadorAtual + 1)%2;
                    await jogadores[jogadorAtual].EnviarMsg(MensagemParaCliente.Vez);
                    await jogadores[jogadorAtual].EnviarMsg(jogadorAtual);
                    await jogadores[(jogadorAtual + 1) % 2].EnviarMsg(MensagemParaCliente.Vez);
                    await jogadores[(jogadorAtual + 1) % 2].EnviarMsg(jogadorAtual);
                }
            }

        }

        public void Desconectar()
        {
            Desconectado = true;
            listener.Stop();
        }

        public void AmigoSaiu(int meuNumero)
        {
            jogadores[(meuNumero + 1) % 2].EnviarMsg(MensagemParaCliente.Amigosaiu);
        }

        public async Task Venceu(int numJogador)
        {
            await jogadores[numJogador].EnviarMsg(MensagemParaCliente.Venceu);
            await jogadores[(numJogador + 1) % 2].EnviarMsg(MensagemParaCliente.Perdeu);

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
