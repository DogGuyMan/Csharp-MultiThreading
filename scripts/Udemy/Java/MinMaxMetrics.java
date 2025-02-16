public class MinMaxMetrics {
    
    // Add all necessary member variables
    private volatile long minData = Long.MAX_VALUE;
    private volatile long maxData = Long.MIN_VALUE;
    /**
     * Initializes all member variables
     */
    public MinMaxMetrics() {
        // Add code here
    }

    /**
     * Adds a new sample to our metrics.
     */
    public synchronized void addSample(long newSample) {
        // Add code here
        if(minData >= newSample) {minData = newSample; return;}
        if(maxData <= newSample) {maxData = newSample; return;}
    }

    /**
     * Returns the smallest sample we've seen so far.
     */
    public long getMin() {
        return minData;
    }

    /**
     * Returns the biggest sample we've seen so far.
     */
    public long getMax() {
        return maxData;
    }
}
