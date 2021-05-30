#pragma once

#pragma unmanaged

void Native_Mat12_Mat12(double* result, const double* a, const double* b)
{
	const size_t X1 = 1;
	const size_t N = 13;

	for (auto i = X1; i < N; i++)
	{
		for (auto j = X1; j < N; j++)
		{
			for (auto k = X1; k < N; k++)
			{
				auto va = *(a + i * N + k);
				auto vb = *(b + k * N + j);
				*(result + i * N + j) += *(a + i * N + k) * *(b + k * N + j);
			}
		}
	}
}