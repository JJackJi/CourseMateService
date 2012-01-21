using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;
using OCRRestService;
using System.Collections.Generic;
using System.IO;

public class ImageParser
{
    public Bitmap RetrieveImage(string StringUrl)
    {
        Bitmap BitMapPic;
        try
        {
            System.Net.WebRequest request =
                System.Net.WebRequest.Create(
                StringUrl);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
                response.GetResponseStream();
            BitMapPic = new Bitmap(responseStream);
            return BitMapPic;

        }
        catch (System.Net.WebException)
        {
            BitMapPic = new Bitmap("");
            return BitMapPic;
        }
    }

	public String Parse(String url)
	{
        string apPath = "";
        String s = "";
        String debugInfo = "";
        try
        {
        double scalefactor = 4.8;
        Bitmap b = RetrieveImage(url);
        Color bgcolor = Color.FromArgb(182, 209, 146);
        int startingx = 0;
        Rectangle[] r = new Rectangle[5];
        int columns = 5;
        for (int i = 0; i < columns; i++)
        {
            for (int y = 0; y < b.Height; y+=10)
            {
                for (int x = startingx; x < b.Width; x += 10)
                {
                    Color col = b.GetPixel(x, y);
                   
                    if (col == bgcolor)
                    {
                        
                        r[i] = getRect(b, x, y);
                        if ((r[i].X) - r[Math.Max(i - 1, 0)].X > 75 + r[i].Width)
                        {
                            columns--;
                        }
                        goto breakout;

                    }
                }
            }
            columns--;
        breakout:
            startingx = r[i].Right + 10 ;
        
        }
        s = "";
        apPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        for (int i = 0; i < columns; i++)
        {
            
            Bitmap r1 = new Bitmap((int)(r[i].Width * scalefactor), (int)(b.Height * scalefactor));
            using (Graphics g = Graphics.FromImage((Image)r1))
                g.DrawImage(b, new Rectangle(0, 0, (int)(r[i].Width * scalefactor), (int)(b.Height * scalefactor)), new Rectangle(new Point(r[i].Location.X, 0), new Size(r[i].Width, b.Height)), GraphicsUnit.Pixel);
            r1.Save(apPath + i + ".png", ImageFormat.Png);
            r1.Dispose();
            
                Process p = new Process();
                
                p.StartInfo.WorkingDirectory = apPath;
                p.StartInfo.FileName = apPath + "Tesseract-OCR/tesseract.exe";
                p.StartInfo.Arguments = apPath + i + ".png " + apPath + i;
                p.Start();
                p.WaitForExit();

                debugInfo += "|||" + p.StartInfo.Arguments;
            
                System.IO.StreamReader sr = new System.IO.StreamReader(apPath + i + ".txt");
                s += "\n" + sr.ReadToEnd();
                sr.Close();
            
        }
        s = s.Replace("~", "-");
        s = s.Replace("I-I", "H");
        s = s.Replace("!-I", "H");
        s = s.Replace("I-!", "H");
        s = s.Replace("'l'", "T");
        s = s.Replace("'l’", "T");
        s = s.Replace("‘l'", "T");
        s = s.Replace("‘l’", "T");
        s = s.Replace("9\"", "PM");
        s = s.Replace("\"", "M");
        s = s.Replace("I-l", "H");
        s = s.Replace("$", "5");
        s = s.Replace("l'l", "H");
        s = s.Replace("l'I", "H");
        s = s.Replace("\\N", "W");
        s = s.Replace("alI", "all");
        s = s.Replace("IIATH", "MATH");
        s = s.Replace("HATH", "MATH");
        s = s.Replace("HAT\"", "MATH");
        s = s.Replace("IIATII", "Math");
        s = s.Replace("MAUI", "Math");
        s = s.Replace("OO1", "001");
        s = s.Replace("O01", "001");
        s = s.Replace("Colltts", "Coutts");

        s = s.Replace("0AH", "0AM");
        s = s.Replace("—", "-");

        for (int i = 0; i < 10; i++) { s = s.Replace("Z" + i, "2" + i); }
        
        b.Dispose();
       }
            catch (Exception ex)
            {
                String outstr = "";
                foreach (String str in Directory.EnumerateFiles(apPath))
                {
                    outstr += str + "   ";
                }
                foreach (String str in Directory.EnumerateFiles(apPath+ "Tesseract-OCR/"))
                {
                    outstr += str + "   ";
                }
                return ex.ToString() + outstr + debugInfo;
            }
        return getJSON(s);
        
	}

    private String getJSON(String s)
    {
        List<SampleItem> SampleResultList = new List<SampleItem>();
        String[] lines = s.Split(new String[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
        List<String> lineArray = new List<string>();
        lineArray.AddRange(lines);

        String numbers = "0123456789";
        String letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int mod = 0;
        for (int i = 0; i < lineArray.Count;)
        {
            // Remove lines we don't want
            Boolean found = false;
            string current = lineArray[i];
            for (int a = 0; a < current.Length - 2; a++)
            {
                if (mod == 0 || mod == 2)
                {
                    if (numbers.Contains(current[a].ToString()) &&
                        numbers.Contains(current[a + 1].ToString()) &&
                        numbers.Contains(current[a + 2].ToString()))
                    {
                        found = true;
                    }
                }
                if (mod == 1)
                {
                    if (current[a].Equals(':') &&
                        numbers.Contains(current[a + 1].ToString()) &&
                        numbers.Contains(current[a + 2].ToString()))
                    {
                        found = true;
                    }
                }
            }
            if (!found) lineArray.RemoveAt(i);
            else
            {
                i++;
                mod =(mod + 1) % 3;
            }

            
        }
        List<String> courses = new List<String>();
        for (int i = 0; i < lineArray.Count; i += 3)
        {
            if (!courses.Contains(lineArray[i]))
            {
                courses.Add(lineArray[i]);
            }

        }
        for (int i = 0; i < courses.Count; i++)
        {
            List<int> indices = new List<int>();
            for (int x = 0; x < lineArray.Count; x++ )
            {
                if (lineArray[x].Equals(courses[i]))
                    indices.Add(x);
            }
            SampleItem SampleItem = new SampleItem();
            SampleItem.CourseName = courses[i];
            SampleItem.CourseLocation = lineArray[indices[0]+2];
            SampleItem.CourseTime = new List<string>();

            for (int j = 0; j < indices.Count; j++)
            {
                SampleItem.CourseTime.Add(lineArray[indices[j]+1]);
            }

            SampleResultList.Add(SampleItem);
        }
        
        JsonOutput JsonOutput = new JsonOutput();

        string JsonResult = JsonOutput.ParseToJson(SampleResultList);

        return JsonResult;
    }



    private Rectangle getRect(Bitmap b, int x, int y)
    {
        int x1 = x; int y1 = y;
        Color c = Color.FromArgb(255, 255, 255, 255);
        Color c2 = Color.FromArgb(255, 95, 154, 35);
        Color cCurrent = Color.FromArgb(0, 0, 0, 0);
        while (cCurrent != c2 && cCurrent != c) { cCurrent = b.GetPixel(--x1, y1); }
        int left = x1;
        x1 = x; y1 = y;
        cCurrent = Color.FromArgb(0, 0, 0, 0);
        while (cCurrent != c2 && cCurrent != c) { cCurrent = b.GetPixel(x1, --y1); }
        int top = y1;
        x1 = x; y1 = y;
        cCurrent = Color.FromArgb(0, 0, 0, 0);
        while (cCurrent != c2 && cCurrent != c) { cCurrent = b.GetPixel(++x1, y1); }
        int right = x1;
        x1 = x; y1 = y;
        cCurrent = Color.FromArgb(0, 0, 0, 0);
        while (cCurrent != c2 && cCurrent != c) { cCurrent = b.GetPixel(x1, ++y1); }
        int bottom = y1;
        return new Rectangle(left, top, right - left, bottom - top);
    }
}
