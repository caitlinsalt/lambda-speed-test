# Lambda Speed Test

This started out as quick experiment to see if&mdash;on versions of .NET that support it&mdash;static lambda calls are noticeably faster than instance lambda calls.

The app runs the same simple arithmetic calculation iteratively on variously-sized data sets of random integers, and reports the time taken for each to complete.  Optionally, it can repeat each test multiple times and will then average the results.  The arithmetic calculation is called in various different ways, so the speed of each coding pattern can be compared.

The data sets range from one item to 1,000,000,000 items.  The app requires just over 8Gb of memory, so requires at least this much memory available in order to not distort the results due to swapping. 

At present the following coding patterns are tested:
- Calling a virtual method from inside a `foreach` loop.
- Calling a non-virtual instance method from inside a `foreach` loop.
- Calling a static method from inside a `foreach` loop.
- Calling an instance lambda method from inside a `foreach` loop.
- Calling a static lambda method from inside a `foreach` loop.
- Calling an instance lambda method using a LINQ `Select()` call.
- Calling a static lambda method using a LINQ `Select()` call.

## Usage

Running the app with no arguments runs each test once.

The `-c` (or `--count`) option sets the number of times to repeat each test.  The average time taken for each size of test will be shown.

By default the app displays a "twirling baton" when tests are running.  The baton only updates between tests, so that its display does not affect test results.  The `-q` or `--quiet` option stops the baton being displayed, so that output can be redirected to a file.
