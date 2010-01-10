
print("1" + (1 + 2));

print(1 - 4);
print(2 * 5);
print(Infinity * 0);
print(Infinity * Infinity);
print(0 / 1);
print(10 % 3);
print(10 % NaN);

foo = 2;

print(foo += 1);
print(foo -= 1);
print(foo *= 1);
print(foo /= 1);
print(foo %= 1);

print(1 < 1);
print(1 > 1);
print(1 <= 1);
print(1 >= 1);

foo = 3;
print(foo &= 1);
print(foo |= 1);
print(foo ^= 1);

print(1 == 1);
print(1 != 1);

print(1 && 2);
print(0 || 3);

print(1 | 2);
print(1 & 2);
print(1 ^ 2);

print(1 << 2);
print(5 >> 2);

foo = 4;
print(foo <<= 1);
print(foo >>= 1);

print(~1);
print(!1);
print(-1);
print(+"3,14");

foo = 5;

print(foo++);
print(foo--);
print(++foo);
print(--foo);

print(typeof "asd");
print(typeof 123);
print(typeof undefined);
print(typeof null);
print(typeof {});
print(typeof function () { });
print(typeof true);

function foo () {
    print("void-test:");
};

print(void foo());


foo = 4;
bar = 4;

if (foo > bar) {
    print(foo + " > " + bar);
} else {
    print(foo + " <= " + bar);
}


print(foo > bar ? "foo>bar" : "foo<=bar");

print("1" == 1); // true

print("1" === 1); // false
print(1 === 2); // false
print(1 === 1); // true

print("1" !== 1); // true
print(1 !== 2); // true
print(1 !== 1); // false

print(-14 >>> 2)

foo = -14

print(foo >>>= 2)

/*
//foo = 1 === 1
//foo = 1 !== 1
*/
