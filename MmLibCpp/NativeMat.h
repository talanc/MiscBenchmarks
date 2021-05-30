#pragma once

#pragma unmanaged

void Native_Mat12_Mat12(double* result, const double* a, const double* b)
{
	const size_t X1 = 1;
	const size_t N = 13;

	for (auto i = X1; i < N; i++)
	{
		auto in = i * N;
		for (auto j = X1; j < N; j++)
		{
			auto r = result + in + j;
			for (auto k = X1; k < N; k++)
			{
				*r += *(a + i * N + k) * *(b + k * N + j);
			}
		}
	}
}
