java -jar grammatica-1.6.jar language.grammar --debug 
java -jar grammatica-1.6.jar language.grammar --tokenize test.def > tokens_test.txt
java -jar grammatica-1.6.jar language.grammar --parse test.def > parse_test.txt
pause