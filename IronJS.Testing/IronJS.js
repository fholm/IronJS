

try {
    throw exc;
} catch (e) {
    print("exception caught");
    print("try ... catch");
}

print("--------------------------");

try {
    throw exc;
} catch (e) {
    print("exception caught");
} finally {
    print("try ... catch ... finally");
}

print("--------------------------");

try {
    //throw exc;
} finally {
    print("try ... finally");
} 