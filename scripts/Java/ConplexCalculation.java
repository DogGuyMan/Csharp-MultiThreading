import java.io.*;
import java.math.BigInteger;
import java.util.*;

public class ComplexCalculation {
    public BigInteger calculateResult(BigInteger base1, BigInteger power1, BigInteger base2, BigInteger power2){
        BigInteger result = BigInteger.ZERO;
        List<PowerCalculatingThread> PCthreads = new ArrayList<>();
        PCthreads.add(new PowerCalculatingThread(base1, power1));
        PCthreads.add(new PowerCalculatingThread(base2, power2));
        for(Thread t : PCthreads) {
            t.start();
        }
        for(Thread t : PCthreads) {
            try{
                t.join();
            }catch (InterruptedException e) {
                //throw new RuntimeException(e);
                e.printStackTrace();
            }
        }
        for(PowerCalculatingThread t : PCthreads) {
            result = result.add(t.getResult());
        }
        return result;
    }

    private static class PowerCalculatingThread extends Thread {
        private BigInteger result = BigInteger.ONE;
        private BigInteger base;
        private BigInteger power;

        public PowerCalculatingThread(BigInteger base, BigInteger power) {
            this.base = base;
            this.power = power;
        }

        @Override
        public void run() {
            for(BigInteger i = BigInteger.ZERO; i.compareTo(power) != 0; i = i.add(BigInteger.ONE)){
                result = result.multiply(base);
            }
        }

        public BigInteger getResult() { return result; }
    }

}