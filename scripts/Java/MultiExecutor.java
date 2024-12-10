import java.util.*;

public class MultiExecutor {

    // Add any necessary member variables here
    List<Thread> threadList = new ArrayList<Thread>();
    /* 
     * @param tasks to executed concurrently
     */
    public MultiExecutor(List<Runnable> tasks) {
        // Complete your code here
        for(Runnable r : tasks) {
            threadList.add(new Thread(r));
        }
    }

    /**
     * Starts and executes all the tasks concurrently
     */
    public void executeAll() {
        // complete your code here
        for(Thread t : threadList) {
            t.start();
        }
    }
}