   M S B A M L   `   `   `  ����G  CDrawingTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.UserMajor   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  隐藏外专业工作集��5   	   $	��500��6   $	��350��6'    � 
ResizeMode$ NoResize=�63    � WindowStartupLocation$ CenterScreen=�6a   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��100* �6   �� 5      $	��60* �6   �� 5      $	��60* �6   �� 5      $	��65* �6    � RowDefinitions 5   
   �� 5      $	��90* �6   �� 5      $��9* �6   .+�5   
   �� $��外专业工作集��6   $��0��6#   $��0��63   $��4��6@   $��5,5,5,5q�6T   
�������.��5      � .+�5      �� -   5         ��Name$ WorkSetListBox��5      $��5,5,5,5q�62   $��0q�6C   ��5      �� 5      .R�5      �� $
��Left=�5   '   $	��Top=�6B   $��Center=�5   '   $
��Auto=�6I    � UseLayoutRounding .   d� 	IsChecked 5   '   �� *
IsSelected+��6I   �� *WorkSetName+��6i   �� *No+5   {   5      5      5      5       �� 5"   
   -   5         ��Name$ OkButton��5"      $��确定��6#   $��3��61   $��1��6A   $��	5,5,10,10q�6N   $
��10.5(�6a   	 � Cursor$
	 Hand��6�   ��5#      �� 5$      
 �� 
TargetType"
 ��6"   .R�5%      �� #��z��6   $��1q�6Q    �� CornerRadius$ 3��6e   
�������._�5'      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5(      5)      5+      5-      5.      