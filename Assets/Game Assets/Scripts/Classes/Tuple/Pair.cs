public class Pair<T1, T2> {
	public T1 A { get; private set; }
	public T2 B { get; private set; }
	internal Pair(T1 a, T2 b) {
		A = a;
		B = b;
	}
}

public static class Pair {
	public static Pair<T1, T2> New<T1, T2>(T1 a, T2 b) {
		return new Pair<T1, T2>(a, b);
	}
}
