using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldTransit_Seattle
{
    class A : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    class B : A { }
    class C : B { }
    class D : C { }
    class E : D { }
    class Test
    {
        // The instance of A passed in is not flagged because M1 doesn't take ownership.
        // We are considering transferring ownership when passed to a constructor, or doing
        // deeper analysis (tradeoffs between correctness and performance), etc.
        void M1(A a, string param, string param2)
        {
            A a1 = new A();     // Maybe disposed.
            B a2 = new B();     // Never disposed.
            A b;
            if (param != null)
            {
                a = a1;
                b = new C();     // Never disposed.
            }
            else
            {
                a = a2;
                b = new D();     // Never disposed.
            }

            // a points to either a1 or a2.
            // b points to either instance created in if or else.
            if (param != null)
            {
                A c = new A();
                a = c;
                b = a1;
            }
            else
            {
                C d = new E();
                b = d;
                a = b;      // Track instance through multiple assignments
            }
            a.Dispose();         // a points to either c or d.
            b.Dispose();         // b points to either a1 or d.
        }
    }
}
