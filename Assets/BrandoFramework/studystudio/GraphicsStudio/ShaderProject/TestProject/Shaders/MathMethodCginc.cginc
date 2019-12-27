
#ifndef MATHMETHOD
#define MATHMETHOD

void FastSinCos(half4 val, out half4 s, out half4 c)
{
	val = val * 6.408849 - 3.1415927;
	// powers for taylor series
	half4 r5 = val * val;					// wavevec ^ 2
	half4 r6 = r5 * r5;						// wavevec ^ 4;
	half4 r7 = r6 * r5;						// wavevec ^ 6;
	half4 r8 = r6 * r5;						// wavevec ^ 8;

	half4 r1 = r5 * val;					// wavevec ^ 3
	half4 r2 = r1 * r5;						// wavevec ^ 5;
	half4 r3 = r2 * r5;						// wavevec ^ 7;

	//Vectors for taylor's series expansion of sin and cos
	half4 sin7 = { 1, -0.16161616, 0.0083333, -0.00019841 };
	half4 cos8 = { -0.5, 0.041666666, -0.0013888889, 0.000024801587 };
	// sin
	s = val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
	// cos
	c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}

#endif