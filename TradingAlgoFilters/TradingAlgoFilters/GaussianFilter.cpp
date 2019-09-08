#include <iostream>
#include <cmath>
#include <iomanip>
using namespace std;

// https://www.codewithc.com/gaussian-filter-generation-in-c/
// https://www.geeksforgeeks.org/gaussian-filter-generation-c/

void filter(double gk[][5]); // Declaration of function to create Gaussian filter

int main()
{
    double gk[5][5];
    filter(gk); // Function call to create a filter
    for(int i = 0; i < 5; ++i) // loop to display the generated 5 x 5 Gaussian filter
    {
        for (int j = 0; j < 5; ++j)
            cout<<gk[i][j]<<"\t";
        cout<<endl;
    }
}

void filter(double gk[][5])
{
       double stdv = 1.0;
    double r, s = 2.0 * stdv * stdv;  // Assigning standard deviation to 1.0
    double sum = 0.0;   // Initialization of sun for normalization
    for (int x = -2; x <= 2; x++) // Loop to generate 5x5 kernel
    {
        for(int y = -2; y <= 2; y++)
        {
            r = sqrt(x*x + y*y);
            gk[x + 2][y + 2] = (exp(-(r*r)/s))/(M_PI * s);
            sum += gk[x + 2][y + 2];
        }
    }
 
    for(int i = 0; i < 5; ++i) // Loop to normalize the kernel
        for(int j = 0; j < 5; ++j)
            gk[i][j] /= sum;
 
}
