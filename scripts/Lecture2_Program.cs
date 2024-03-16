// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
namespace Udemy.MultiThreading.Lecture2
{
    public class Program
    {
        public static void Main(string[] args) {
            Major_1.MajorAction();
        }

        public class Major_1() {
            static Thread thread;
            public static void MajorAction()
            {
            }
        }
    }
}


// public class Main1 {
//     public static void main(String [] args) {
//         Thread thread = new Thread(new BlockingTask());

//         thread.start();
//     }

//     private static class BlockingTask implements Runnable {

//         @Override
//         public void run() {
//             //do things
//             try {
//                 Thread.sleep(500000);
//             } catch (InterruptedException e) {
//                 System.out.println("Existing blocking thread");
//             }
//         }
//     }
// }

// public class Main2 {

//     public static void main(String[] args) {
//         Thread thread = new Thread(new LongComputationTask(new BigInteger("200000"), new BigInteger("100000000")));

//         thread.start();
//         thread.interrupt();
//     }

//     private static class LongComputationTask implements Runnable {
//         private BigInteger base;
//         private BigInteger power;

//         public LongComputationTask(BigInteger base, BigInteger power) {
//             this.base = base;
//             this.power = power;
//         }

//         @Override
//         public void run() {
//             System.out.println(base + "^" + power + " = " + pow(base, power));
//         }

//         private BigInteger pow(BigInteger base, BigInteger power) {
//             BigInteger result = BigInteger.ONE;

//             for (BigInteger i = BigInteger.ZERO; i.compareTo(power) != 0; i = i.add(BigInteger.ONE)) {
//                 if (Thread.currentThread().isInterrupted()) {
//                     System.out.println("Prematurely interrupted computation");
//                     return BigInteger.ZERO;
//                 }
//                 result = result.multiply(base);
//             }

//             return result;
//         }
//     }
// }
