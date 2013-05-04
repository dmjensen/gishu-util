Steps:
1. Extract all jars inside ..\..\libs\jmock-2.5.1\jmock-2.5.1-jars.zip to the same parent folder. All extracted jars should be alongside the original zip file.

2. Set the classpath to include all the relevant jars

\LearnJMock>set classpath=.;..\..\libs\jUnit-4.8.1\junit-4.8.1.jar;..\..\libs\jmock-2.5.1\jmock-2.5.1.jar;..\..\libs\jmock-2.5.1\hamcrest-core-1.1.jar;..\..\libs\jmock-2.5.1\hamcrest-library-1.1.jar;..\..\libs\jmock-2.5.1\jmock-junit4-2.5.1.jar

3. Compile
> javac *.java

4. Run JUnit to run the tests
> java org.junit.runner.JUnitCore TestBakery

(I've tested this on Java 1.5)

Gishu Pillai
gishu@hotmail.com