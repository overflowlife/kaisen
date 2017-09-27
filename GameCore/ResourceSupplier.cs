using KaisenLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    /// <summary>
    /// クラス間（およびアセンブリ間）で共有する必要のあるリソースの集合体です。
    /// </summary>
   public class ResourceSupplier
    {
        public Logger Logger { get; private set; }
        public Messenger Messenger { get; private set; }
        public Game Game { get; private set; }

        public ResourceSupplier()
        {
            Logger = null;
            Messenger = null;
            Game = null;
        }

        public void Inject(Logger logger) => Logger = logger;
        public void Inject(Messenger messenger) => Messenger = messenger;
        public void Inject(Game game) => Game = game;

    }
}
