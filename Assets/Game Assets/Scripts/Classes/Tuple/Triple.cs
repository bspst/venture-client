public class Triple<T1, T2, T3> {
	public T1 A { get; private set; }
	public T2 B { get; private set; }
	public T3 C { get; private set; }
	internal Triple(T1 a, T2 b, T3 c) {
		A = a;
		B = b;
		C = c;
	}
}

public static class Triple {
	public static Triple<T1, T2, T3> New<T1, T2, T3>(T1 a, T2 b, T3 c) {
		return new Triple<T1, T2, T3>(a, b, c);
	}
}