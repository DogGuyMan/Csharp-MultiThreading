using Udemy;
using Udemy.MultiThreading;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    // args는 0부터 시작한다.
    public static void Main(string[] args) {
        for(int i = 0; i < args.Length; i++) {
            Console.WriteLine(args[i]);
        }
        if( args.Length == 0 || args[0] == "Lecture") {
            Console.WriteLine("Lecture Run");
            // Udemy.MultiThreading.Lecture2.Major_1.MajorAction();
            // Inflearn.MultiThreading.Lecture2.ThreadStartJoin.MajorAction(args);
            // MyExtra.MultiThreading.IOOnWorkerThread.Major.MajorAction();
            // MyExtra.MultiThreading.IOOnIOThread.Major.MajorAction();
            // Inflearn.MultiThreading.Lecture4.GuardedSuspension.MajorAction();
        }
        else if(args[0] == "Test") {
            Console.WriteLine("Test Run");
        }
    }
}