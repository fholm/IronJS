
print("Testing for(i = 0; i < 5; ++i)");
for (i = 0; i < 5; ++i) {
    print(i);
}

print("Testing for(; i < 5; ++i)");
i = 0;
for (; i < 5; ++i) {
    print(i);
}

print("Testing for(i = 0; i < 5; )");
for (i = 0; i < 5; ) {
    print(i);
    ++i;
}

print("Testing for(; i < 5; )");
i = 0;
for (; i < 5;) {
    print(i);
    ++i;
}

print("Testing for(var i = 0; i < 5; ++i)");
for (var i = 0; i < 5; ++i) {
    print(i);
}

print("Testing for(var i = 0; i < 5;)");
for (var i = 0; i < 5; ) {
    print(i);
    ++i;
}
