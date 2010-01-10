
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

var n = 10;
foo = function () {
    --n;
    return n > 0;
};

n = 10000;

while (n > 0) {
    --n;
}

i = 0;

print("-------");

for (i = 0; i < 2; ++i) {
    print(i);
}

print("-------");

for (; i < 4; ++i) {
    print(i);
}

print("-------");

for (i = 0; i < 8; ) {
    print(i);
    ++i;
}

print("-------");

i = 0;
for ( ; i < 16; ) {
    print(i);
    ++i;
}

print("-------");


for(i = 0; i < 10; ++i) {
    if(i > 4)
    {
        print("breaking on: " + i);
        break;
    }
    print(i);
}

// nested break
for (i = 0; i < 10; ++i) {

    if (i > 3) {
        print("breaking outer");
        break;
    }

    print("outer:" + i);

    for (j = 0; j < 5; ++j) {
        if (j > 2) {
            print("break inner");
            break;
        }
        
        print("inner:"+j);
    }
}


/* WORKS, but loops forever
i = 0;
for (;;) {
    print(i);
    ++i;
}
*/


/*
bar_lbl:
while (true) {
    break bar_lbl;
}
*/

/*
//foo = 1 === 1
//foo = 1 !== 1
*/
