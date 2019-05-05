using System.Collections.Generic;
using System.Web.Mvc;
using OW.Experts.WebUI.ViewModels.Admin;

namespace OW.Experts.WebUI.Infrastructure.Binders
{
    public class NodeCandidatesBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindindContext)
        {
            if (bindindContext.ModelType == typeof(NodeCandidateListViewModel)) {
                var request = controllerContext.HttpContext.Request;
                var nodeCandidateListViewModel = new NodeCandidateListViewModel
                {
                    NodeCandidates = new List<NodeCandidateViewModel>()
                };
                for (var nodeI = 0; nodeI < request.Form.Count / 5; nodeI++) {
                    nodeCandidateListViewModel.NodeCandidates.Add(
                        new NodeCandidateViewModel
                        {
                            Notion = request.Form.Get($"model.NodeCandidates[{nodeI}].Notion"),
                            TypeId = request.Form.Get($"model.NodeCandidates[{nodeI}].TypeId"),
                            IsSaveAsNode = bool.Parse(
                                request.Form.Get($"model.NodeCandidates[{nodeI}].IsSaveAsNode").Split(',')[0]),
                            ExpertCount = int.Parse(
                                request.Form.Get($"model.NodeCandidates[{nodeI}].ExpertCount")),
                            TotalExpert = int.Parse(
                                request.Form.Get($"model.NodeCandidates[{nodeI}].TotalExpert"))
                        });
                }

                return nodeCandidateListViewModel;
            }

            return base.BindModel(controllerContext, bindindContext);
        }
    }
}