using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;

namespace AkkaCS {

    public class MyAkkaSystem
    {
        private ActorSystem actorSystem;

        public MyAkkaSystem() {
            actorSystem = ActorSystem.Create("my-akka-system");

            //var counter1 = actorSystem.ActorOf<CounterActor>("counter1");
            var counter2 = actorSystem.ActorOf(Props.Create<CounterActor>().WithRouter(FromConfig.Instance), "counter2");

            counter2.Tell(1);
            counter2.Tell(11);
            counter2.Tell(21);
            counter2.Tell(31);
            counter2.Tell(41);
        }
    }

    public class CounterActor : ReceiveActor {
        public CounterActor() {
            Receive<int>(
                i => {                    
                    DoWork(i);                    
                }
                );
        }
        
        private void DoWork(int i) {
            var j = i + 5;
            while (i < j) {
                Console.WriteLine(i++);
                Thread.Sleep(500);
            }
        }

        
    }
}
