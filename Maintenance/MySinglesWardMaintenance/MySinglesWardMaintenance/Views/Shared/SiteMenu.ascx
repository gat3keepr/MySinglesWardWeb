<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<!-- -36 / 69-->
<!--  start nav-outer-repeat................................................................................................. START -->
<div class="nav-outer-repeat">
    <!--  start nav-outer -->
    <div class="nav-outer">
        <!-- start nav-right -->
        <div id="nav-right">
        
            </div>
            <!--  end account-content -->
        </div>
        <!-- end nav-right -->
        <!--  start nav -->
        <div class="nav">
            <div class="table">
                       <%  Html.RenderPartial("Menu");%>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <!--  start nav -->
    </div>
    <div class="clear">
    </div>
    <!--  start nav-outer -->
</div>
<!--  start nav-outer-repeat................................................... END -->
<div class="clear">
</div>
