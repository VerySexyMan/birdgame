   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	     � Title$  
AngryBirds��5   	   $	��500��6   $
��1000��6)   .��5	      �� -   5        $��MyCanvas��5	      |�. 
���   �.+�5   
   �� -   5        $��player��5      $��35��6"   $��35��6.   
���  �� �� Left$	 100��6D   
 �� Top$	 280��6V   �� 5   
   -   5        $��	scoreText��5      $��Score: 0��6!   $��18(�64   $
��Bold"�6B   
�������$��Center=�6g   $ 86��6�   $	��Top=�6�   �� 5   
   -   5        $��
missedText��5      $��	Missed: 0��6"   $��18(�66   $
��Bold"�6D   
�������$	 901��6i   $��Center=�6{   $��Center=�6�   �� 5   
   -   5        $��Angle��5      $	��100��5       �� Orientation$ 
Horizontal=�6   $��Center=�6(    � Value$ 50Q�5       � Minimum$ 0Q�6    � Maximum$ 90Q�6    � SmallChange$ 1Q�5       � LargeChange$ 10Q�6   	 �� TickPlacement$	 BottomRight=�5      
 �� TickFrequency$
 10Q�6   $ 20��62   $
 420 ��6C   $	��Top=�5      �� 5   
   -   5        $��Speed��5      $	��100��5      $ 
Horizontal=�6   $
��Left=�6(   $ 20Q�5      $ 0Q�6   $ 40Q�6   $ 1Q�5      $ 5Q�6   $	 BottomRight=�5      $
 10Q�6   $	 125��62   $
 420 ��6D   $��Center=�6V   5      5       