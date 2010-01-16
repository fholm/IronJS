
print("Testing while(...) { ... } ");
i = 0;
while (i < 5) {
    print(i);
    ++i;
}

print("Testing do { ... } while(...);");
i = 0;
do {
    print(i);
    ++i;
} while (i < 0);