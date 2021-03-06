// ----------------------------------------------------------------------------
// Zoom Search Engine 3.0 (5/4/2004)
//
// This file (search.js) is the JavaScript search front-end for client side
// searches using index files created by the Zoom Search Engine Indexer.
//
// email: zoom@wrensoft.com
// www: http://www.wrensoft.com
//
// Copyright (C) Wrensoft 2000-2003
//
// This script performs client-side searching with the index data file
// (zoom_index.js) generated by the Zoom Search Engine Indexer. It allows you
// to run searches on mediums such as CD-ROMs, or other local data, where a
// web server is not available.
//
// We recommend against using client-side searches for online websites because
// it requires the entire index data file to be downloaded onto the user's
// local machine. This can be very slow for large websites, and our server-side
// search scripts (search.php and search.asp) are far better suited for this.
// However, JavaScript is still an option for smaller websites in a limited
// hosting situation (eg: no php or asp)
// ----------------------------------------------------------------------------

// ----------------------------------------------------------------------------
// Settings (change if necessary)
// ----------------------------------------------------------------------------

// The options available in the dropdown menu for number of results
// per page
var PerPageOptions = new Array(10, 20, 50, 100);

// Globals
var SkippedWords = 0;
var searchWords = new Array();
var SkippedOutputStr = "";

// ----------------------------------------------------------------------------
// Helper Functions
// ----------------------------------------------------------------------------

// This function will return the value of a GET parameter
function getParam(paramName)
{
    paramStr = document.location.search;
    if (paramStr == "")
        return "";

    // remove '?' in front of paramStr
    if (paramStr.charAt(0) == "?")
        paramStr = paramStr.substring(1, paramStr.length);

    arg = (paramStr.split("&"));
    for (i=0; i < arg.length; i++) {
        arg_values = arg[i].split("=")
        if (unescape(arg_values[0]) == paramName) {
            if (UseUTF8 == 1 && self.decodeURIComponent) // check if decodeURIComponent() is defined
                ret = decodeURIComponent(arg_values[1]);
            else
                ret = unescape(arg_values[1]);  // IE 5.0 and older does not have decodeURI
            return ret;
        }
    }
    return;
}

// Compares the two values, used for sorting output results
// Results that match all search terms are put first, highest score
function SortCompare (a, b)
{
    if (a[2] < b[2]) return 1;
    else if (a[2] > b[2]) return -1;
    else if (a[1] < b[1]) return 1;
    else if (a[1] > b[1]) return -1;
    else return 0;
}

function pattern2regexp(pattern) 
{
	pattern = pattern.replace(/\#/g, "\\#");
	pattern = pattern.replace(/\$/g, "\\$");
    pattern = pattern.replace(/\./g, "\\.");
    pattern = pattern.replace(/\*/g, ".*");
    pattern = pattern.replace(/\?/g, ".?");
    if (SearchAsSubstring == 1)
        return pattern;

    return "^" + pattern + "$";
}

function HighlightDescription(line) {
    res = " " + line + " ";
    for (i = 0; i < matchwords_num; i++) {
        // replace with marker text, assumes [;:] and [:;] is not the search text...
        // can not use \\b due to IE's regexp bug with foreign diacritic characters
        // treated as non-word characters
        //res = res.replace(new RegExp("([\\s\.\,\:]+)("+matchwords[i]+")([\\s\.\,\:]+)", "gi"), "$1[;:]$2[:;]$3");        
        res = res.replace(new RegExp("(\\W|^)("+matchwords[i]+")(\\W|$)", "gi"), "$1[;:]$2[:;]$3");
    }
    // replace the marker text with the html text
    // this is to avoid finding previous <span>'ed text.    
    res = res.replace(/\[;:\]/g, "<span class=\"highlight\">");
    res = res.replace(/\[:;\]/g, "</span>");
    return res;
}

function SkipSearchWord(sw) {
    if (searchWords[sw] != "") {
        if (SkippedWords > 0)
            SkippedOutputStr += ", ";
        SkippedOutputStr += "\"<b>" + searchWords[sw] + "</b>\"";
        searchWords[sw] = "";
    }
    SkippedWords++;
}


// ----------------------------------------------------------------------------
// Parameters initialisation (globals)
// ----------------------------------------------------------------------------

var query = getParam("zoom_query");
query = query.replace(/[\++]/g, " ");  // replace the '+' with spaces
query = query.replace(/[,+]/g, " ");
query = query.replace(/\</g, "&lt;");
query = query.replace(/[\"+]/g, " ");

var per_page = parseInt(getParam("zoom_per_page"));
if (isNaN(per_page)) per_page = 10;

var page = parseInt(getParam("zoom_page"));
if (isNaN(page)) page = 1;

var andq = parseInt(getParam("zoom_and"));
if (isNaN(andq)) andq = 0;

var cat = parseInt(getParam("zoom_cat"));
if (isNaN(cat)) cat = -1;   // search all categories


/*
if (typeof(catnames) != "undefined" && typeof(catpages) != "undefined")
    UseCats = true;
else
    UseCats = false;
*/


var data = new Array();
var output = new Array();

if (Highlighting == 1) {
    var matchwords = new Array();
    var matchwords_num = 0;
}

target = "";
if (UseLinkTarget == 1)
    target = " target=\"" + LinkTarget + "\" ";



// ----------------------------------------------------------------------------
// Main search function starts here
// ----------------------------------------------------------------------------

function ZoomSearch() {

    if (Timing == 1) {
        timeStart = new Date();
    }

    // Display the form
    if (FormFormat > 0) {
        document.writeln("<form method=\"GET\" action=\"" + document.location.href + "\">");
        document.writeln("<input type=\"text\" name=\"zoom_query\" size=\"20\" value=\"" + query + "\">");
        document.writeln("<input type=\"submit\" value=\"Search\">");
        if (FormFormat == 2) {
            document.writeln("<small>Results per page:");
            document.writeln("<select name='zoom_per_page'>");
            for (i = 0; i < PerPageOptions.length; i++) {
                document.write("<option");
                if (PerPageOptions[i] == per_page)
                    document.write(" selected=\"selected\"");
                document.writeln(">" + PerPageOptions[i] + "</option>");
            }
            document.writeln("</select></small></p>");
            if (UseCats) {
                document.write("Category: ");
                document.write("<select name='zoom_cat'>");
                // 'all cats option
                document.write("<option value=\"-1\">All</option>");
                for (i = 0; i < catnames.length; i++) {
                    document.write("<option value=\"" + i + "\"");
                    if (i == cat)
                        document.write(" selected=\"selected\"");
                    document.writeln(">" + catnames[i] + "</option>");
                }
                document.writeln("</select>&nbsp;&nbsp;");
            }

            document.writeln("<small>Match: ");
            if (andq == 0) {
                document.writeln("<input type=\"radio\" name=\"zoom_and\" value=\"0\" checked>any search words");
                document.writeln("<input type=\"radio\" name=\"zoom_and\" value=\"1\">all search words");
            } else {
                document.writeln("<input type=\"radio\" name=\"zoom_and\" value=\"0\">any search words");
                document.writeln("<input type=\"radio\" name=\"zoom_and\" value=\"1\" checked>all search words");
            }
            document.writeln("</small>");
        }
        document.writeln("</form>");
    }

    // give up early if no search words provided
    if (query.length == 0) {
        //document.writeln("No search query entered.<br>");
        if (ZoomInfo == 1)
            document.writeln("<center><small>Search powered by <a href=\"http://www.wrensoft.com/zoom\"><b>Zoom Search Engine</b></a></small><br></center>");
        return;
    }

    if (MapAccents == 1) {
        for (i = 0; i < NormalChars.length; i++) {
            query = query.replace(AccentChars[i], NormalChars[i]);
        }
    }


	if (ToLowerSearchWords == 1)
    	query = query.toLowerCase();

	// prepare search query, strip quotes, trim whitespace            
    if (WordJoinChars.indexOf(".") == -1)
    	query = query.replace(/[\.+]/g, " ");
    
    if (WordJoinChars.indexOf("-") == -1)
    	query = query.replace(/[\-+]/g, " ");

    if (WordJoinChars.indexOf("_") == -1)
    	query = query.replace(/[\_+]/g, " ");

    if (WordJoinChars.indexOf("'") == -1)
    	query = query.replace(/[\'+]/g, " ");

    if (WordJoinChars.indexOf("#") == -1)
    	query = query.replace(/[\#+]/g, " ");

    if (WordJoinChars.indexOf("$") == -1)
    	query = query.replace(/[\$+]/g, " ");

	//    if (WordJoinChars.indexOf(",") == -1)
  	//  	query = query.replace(/[\,+]/g, " ");

    // split search phrase into words
    searchWords = query.split(" "); // split by spaces.

    document.write("<div class=\"searchheading\">Search results for \"" + query + "\"");
    if (UseCats) {
        if (cat == -1)
            document.writeln(" in all categories");
        else
            document.writeln(" in category \"" + catnames[cat] + "\"");
    }
    document.writeln("</div><br>");
    
    document.writeln("<div class=\"results\">");

    numwords = searchWords.length;
    kw_ptr = 0;
    outputline = 0;
    usewildcards = 0;
    ipage = 0;
    matches = 0;
    var SWord;
    pagesCount = urls.length;

    // Initialise a result table the size of all pages
    res_table = new Array(pagesCount);
    for (i = 0; i < pagesCount; i++)
    {
        res_table[i] = new Array(2);
        res_table[i][0] = 0;
        res_table[i][1] = 0;
    }

    if (skipwords) {
        for (sw = 0; sw < numwords; sw++) {
            // check min length
            if (searchWords[sw].length < MinWordLen) {
                SkipSearchWord(sw);
                continue;
            }
            // check skip word list
            for (i = 0; i < skipwords.length; i++) {
                if (searchWords[sw] == skipwords[i]) {
                    SkipSearchWord(sw);
                    break;
                }
            }
        }
    }
    if (SkippedWords > 1)
        document.writeln("<i>The following words are in the skip word list, and have been omitted from your search: " + SkippedOutputStr + ".</i><br><br>");
    else if (SkippedWords > 0)
        document.writeln("<i>The word " + SkippedOutputStr + " is in the skip word list, and has been omitted from your search.</i><br><br>");


    // Begin searching...
    for (sw = 0; sw < numwords; sw++) {

        if (searchWords[sw] == "")
            continue;

        if (searchWords[sw].indexOf("*") == -1 && searchWords[sw].indexOf("?") == -1) {
            UseWildCards = 0;
        } else {
            UseWildCards = 1;
            re = new RegExp(pattern2regexp(searchWords[sw]), "g");
        }

        for (kw_ptr = 0; kw_ptr < keywords.length; kw_ptr++) {

            data = keywords[kw_ptr].split(",");

            if (UseWildCards == 0) {
                if (SearchAsSubstring == 0)
                   	//match_result = data[0].search("^" + SWord + "$");
                   	if (data[0] == searchWords[sw])              		
                		match_result = 0;
                	else
                		match_result = -1;              		                    
                else
                    match_result = data[0].indexOf(searchWords[sw]);
            } else
                match_result = data[0].search(re);


            if (match_result != -1) {
                // keyword found, include it in the output list

                if (Highlighting == 1) {
                    // Add to matched words list
                    // Check if its already in the list
                    for (i = 0; i < matchwords.length && matchwords[i] != data[0]; i++);
                    if (i == matchwords.length) {
                        // not in list
                        matchwords_num = matchwords.push(data[0]);
                        matchwords[i] = matchwords[i].replace(/\#/g, "\\#");
						matchwords[i] = matchwords[i].replace(/\$/g, "\\$");                        
                        if (matchwords.length >= HighlightLimit) {
                            Highlighting = 0;
                            document.writeln("<small>Too many words to highlight. Highlighting disabled.</small><br><br>");
                        }
                    }
                }

                for (kw = 1; kw < data.length; kw += 2) {
                    // check if page is already in output list
                    pageexists = 0;
                    ipage = data[kw];
                    if (res_table[ipage][0] == 0) {
                        matches++;
                        res_table[ipage][0] += parseInt(data[kw+1]);
                    }
                    else {

                        if (res_table[ipage][0] > 10000) {
                            // take it easy if its too big to prevent gigantic scores
                            res_table[ipage][0] += 1;
                        } else {
                            res_table[ipage][0] += parseInt(data[kw+1]); // add in score
                            res_table[ipage][0] *= 2;           // double score as we have two words matching
                        }
                    }
                    res_table[ipage][1] += 1;
                }
                if (UseWildCards == 0 && SearchAsSubstring == 0)
                    break;    // this search word was found, so skip to next

            }
        }

    }

    // Count number of output lines that match ALL search terms
    oline = 0;
    fullmatches = 0;
    ResFiltered = false;
    output = new Array();
    var full_numwords = numwords - SkippedWords;
    for (i = 0; i < pagesCount; i++) {
        IsFiltered = false;
        if (res_table[i][0] != 0) {
            if (UseCats && cat != -1) {
                // using cats and not doing an "all cats" search
                if (catpages[i] != cat) {
                    IsFiltered = true;
                }
            }
            if (IsFiltered == false) {
                if (res_table[i][1] >= full_numwords) {
                    fullmatches++;
                } else {
                    if (andq == 1)
                        IsFiltered = true;
                }
            }
            if (IsFiltered == false) {
                // copy if not filtered out
                output[oline] = new Array(3);
                output[oline][0] = i;
                output[oline][1] = res_table[i][0];
                output[oline][2] = res_table[i][1];
                oline++;
            } else {
                ResFiltered = true;
            }
        }
    }
    if (ResFiltered == true)
        matches = output.length;

    // Sort results in order of score, use "SortCompare" function
    if (matches > 1)
        output.sort(SortCompare);

    //Display search result information
    document.writeln("<div class=\"summary\">");
    if (matches == 1)
        document.writeln("1 result found.<br>");
    else if (matches == 0)
        document.writeln("No results found.<br>");
    else if (numwords > 1 && andq == 0) {
        //OR
        SomeTermMatches = matches - fullmatches;
        document.writeln(fullmatches + " pages found containing all search terms. ");
        if (SomeTermMatches > 0)
            document.writeln(SomeTermMatches + " pages found containing some search terms.");
        document.writeln("<br>");
    }
    else if (numwords > 1 && andq == 1) //AND
        document.writeln(fullmatches + " pages found containing all search terms.<br>");
    else
        document.writeln(matches + " results found.<br>");

    document.writeln("</div>\n");

    // number of pages of results
    num_pages = Math.ceil(matches / per_page);
    if (num_pages > 1)
        document.writeln("<br>" + num_pages + " pages of results.<br>\n");

    // determine current line of result from the output array
    if (page == 1) {
        arrayline = 0;
    } else {
        arrayline = ((page - 1) * per_page);
    }

    // the last result to show on this page
    result_limit = arrayline + per_page;

    // display the results
    while (arrayline < matches && arrayline < result_limit) {
        ipage = output[arrayline][0];
        score = output[arrayline][1];
        if (ResultFormat == 0) {
            // basic style
            document.writeln("<p>Page: <a href=\"" + urls[ipage] + "\"" + target + ">" + titles[ipage] + "</a><br>\n");
        } else {
            // descriptive style
            document.writeln("<p><b>" + (arrayline+1) + ".</b>&nbsp;<a href=\"" + urls[ipage] + "\"" + target + ">" + titles[ipage] + "</a>");
            if (UseCats) {
                catindex = catpages[ipage];
                document.writeln("<span class=\"category\">[" + catnames[catindex] + "]</span>");
            }
            document.writeln("<br>");

            document.writeln("<div class=\"description\">");
            if (Highlighting == 1)
                document.writeln(HighlightDescription(descriptions[ipage]));
            else
                document.writeln(descriptions[ipage]);
            document.writeln("...</div>\n");
        }
        document.writeln("<div class=\"infoline\">Terms matched: " + output[arrayline][2] + " Score: " + score + "&nbsp;&nbsp;URL: " + urls[ipage] + "</div></p>\n");
        arrayline++;
    }

    // Show links to other result pages
    if (num_pages > 1) {
        // 10 results to the left of the current page
        start_range = page - 10;
        if (start_range < 1)
            start_range = 1;

        // 10 to the right
        end_range = page + 10;
        if (end_range > num_pages)
            end_range = num_pages;

        document.writeln("<p>Result Pages: ");
        if (page > 1)
            document.writeln("<a href=\"" + document.location.pathname + "?zoom_query=" + query + "&zoom_page=" + (page-1) + "&zoom_per_page=" + per_page + "&zoom_cat=" + cat + "&zoom_and=" + andq + "\">&lt;&lt; Previous</a> ");

        for (i = start_range; i <= end_range; i++) {
            if (i == page) {
                document.writeln(page + " ");
            } else {
                document.writeln("<a href=\"" + document.location.pathname + "?zoom_query=" + query + "&zoom_page=" + i + "&zoom_per_page=" + per_page + "&zoom_cat=" + cat + "&zoom_and=" + andq + "\">" + i + "</a> ");
            }
        }

        if (page != num_pages)
            document.writeln("<a href=\"" + document.location.pathname + "?zoom_query=" + query + "&zoom_page=" + (page+1) + "&zoom_per_page=" + per_page + "&zoom_cat=" + cat + "&zoom_and=" + andq + "\">Next &gt;&gt;</a> ");
    }
	
    document.writeln("</div>");	// end results style tag

    if (Timing == 1) {
        timeEnd = new Date();
        timeDifference = timeEnd - timeStart;
        document.writeln("<br><br><small>Search took: " + (timeDifference/1000) + " seconds</small>\n");
    }

    if (ZoomInfo == 1)
        document.writeln("<center><small><p>Search powered by <a href=\"http://www.wrensoft.com/zoom\"><b>Zoom Search Engine</b></a></p></small></center>");


}

