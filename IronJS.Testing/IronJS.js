try {
    throw exc;
} finally {
    print("try-finally");
} 

try {
    throw exc;
} catch (e) {
    
} finally {
    print("try-catch-finally");
}