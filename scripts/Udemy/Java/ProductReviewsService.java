import java.util.*;
import java.util.concurrent.locks.*;

public class ProductReviewsService {
    private final HashMap<Integer, List<String>> productIdToReviews;
    
    // Create your member variables here
    private ReadWriteLock locker;
    private Lock readLocker; 
    private Lock writeLocker;
    
    public ProductReviewsService() {
        this.productIdToReviews = new HashMap<>();
        locker = new ReentrantReadWriteLock();
        readLocker = locker.readLock();
        writeLocker= locker.writeLock();
    }

    /**
     * Adds a product ID if not present
     * Write Lock
     */
    public void addProduct(int productId) {
        Lock lock = getLockForAddProduct();
        
        lock.lock();
        
        try {
            if (!productIdToReviews.containsKey(productId)) {
                productIdToReviews.put(productId, new ArrayList<>());
            }
        } finally {
            lock.unlock();
        }
    }

    /**
     * Removes a product by ID if present
     * Write Lock
     */
    public void removeProduct(int productId) {
        Lock lock = getLockForRemoveProduct();
        
        lock.lock();
        
        try {
            if (productIdToReviews.containsKey(productId)) {
                productIdToReviews.remove(productId);
            }
        } finally {
            lock.unlock();
        }
    }

    /**
     * Adds a new review to a product
     * Write Lock
     * @param productId - existing or new product ID
     * @param review - text containing the product review
     */
    public void addProductReview(int productId, String review) {
        Lock lock = getLockForAddProductReview();
        
        lock.lock();
        
        try {
            if (!productIdToReviews.containsKey(productId)) {
                productIdToReviews.put(productId, new ArrayList<>());
            }
            productIdToReviews.get(productId).add(review);
        } finally {
            lock.unlock();
        }
    }

    /**
     * Returns all the reviews for a given product
     * ReadLock
     */
    public List<String> getAllProductReviews(int productId) {
        Lock lock = getLockForGetAllProductReviews();
        
        lock.lock();
        
        try {
            if (productIdToReviews.containsKey(productId)) {
                return Collections.unmodifiableList(productIdToReviews.get(productId));
            }
        } finally {
            lock.unlock();
        }
        
        return Collections.emptyList();
    }

    /**
     * Returns the latest review for a product by product ID
     * ReadLock Nullable
     */
    public Optional<String> getLatestReview(int productId) {
        Lock lock = getLockForGetLatestReview();
        
        lock.lock();
        
        try {

            if (productIdToReviews.containsKey(productId) && !productIdToReviews.get(productId).isEmpty()) {
                List<String> reviews = productIdToReviews.get(productId);
                return Optional.of(reviews.get(reviews.size() - 1));
            }
        } finally {
            lock.unlock();
        }
        
        return Optional.empty();
    }

    /**
     * Returns all the product IDs that contain reviews
     * ReadLock
     */
    public Set<Integer> getAllProductIdsWithReviews() {
        Lock lock = getLockForGetAllProductIdsWithReviews();
        
        lock.lock();
        
        try {
            Set<Integer> productsWithReviews = new HashSet<>();
            for (Map.Entry<Integer, List<String>> productEntry : productIdToReviews.entrySet()) {
                if (!productEntry.getValue().isEmpty()) {
                    productsWithReviews.add(productEntry.getKey());
                }
            }
            return productsWithReviews;
        } finally {
            lock.unlock();
        }
    }

    Lock getLockForAddProduct() {
        // Add code here
        return writeLocker;
    }

    Lock getLockForRemoveProduct() {
        // add code here
        return writeLocker;
    }

    Lock getLockForAddProductReview() {
        // add code here
        return writeLocker;
    }

    Lock getLockForGetAllProductReviews() {
        // add code here
        return readLocker;
    }

    Lock getLockForGetLatestReview() {
        // add code here
        return readLocker;
    }
    
    Lock getLockForGetAllProductIdsWithReviews() {
        // add code here
        return readLocker;
    }
}
