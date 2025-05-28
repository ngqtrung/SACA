def count_last_digit_five(a, n):
    # Begin your statements here
    
    # End your statements
#End def count_last_digit_five 

# DO NOT ADD NEW OR CHANGE STATEMENTS IN THIS FUNCTION
def input_array():
    n = int(input("Please enter the size of the array: "))
    a = []
    for i in range(n):
        a.append(int(input(f"a[{i}]=")))
    return a, n
#End def input_array
#=====================================================
#=============DO NOT ADD NEW OR CHANGE THE STATEMENTS IN THE MAIN FUNCTION========    
def main():
    print("\nTEST Q3 (2.5 marks):")
    a, n = input_array()
    s = count_last_digit_five(a, n)
    #=====DO NOT ADD NEW OR CHANGE STATEMENTS AFTER THIS LINE=====
    #==THE OUTPUT AFTER THIS LINE WILL BE USED TO MARK YOUR PROGRAM==
    print("OUTPUT:")
    print(s)
#End def main

if __name__ == "__main__":
    main()
#====================================================================================